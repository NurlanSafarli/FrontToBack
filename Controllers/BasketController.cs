using FronyToBack.DAL;
using FronyToBack.Models;
using FronyToBack.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Claims;

namespace FronyToBack.Controllers
{
    public class BasketController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IEmailService _emailService;

        public BasketController(AppDbContext context, UserManager<AppUser> userManager, IEmailService emailService)
        {
            _context = context;
            _userManager = userManager;
            _emailService = emailService;
        }

        public async Task<IActionResult> Index()
        {
            List<BasketItemVM> items = new List<BasketItemVM>();
            if (User.Identity.IsAuthenticated)
            {

                AppUser? user = await _userManager.Users
                    .Include(x => x.BasketItems.Where(bi => bi.OrderId == null))
                    .ThenInclude(bi => bi.Product)
                    .ThenInclude(p => p.ProductImages.Where(pi => pi.IsPrimary == true))
                    .FirstOrDefaultAsync(u => u.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));
                foreach (BasketItem basketitem in user.BasketItems)
                {

                    items.Add(new BasketItemVM
                    {
                        Id = basketitem.ProductId,
                        Price = basketitem.Product.Price,
                        Count = basketitem.Count,
                        Name = basketitem.Product.Name,
                        SubTotal = basketitem.Count * basketitem.Product.Price,
                        Image = basketitem.Product.ProductImages.FirstOrDefault()?.Url

                    }); ;

                }
            }
            else
            {
                if (Request.Cookies["Basket"] is not null)
                {
                    List<BasketCookieItemVM> cookies = JsonConvert.DeserializeObject<List<BasketCookieItemVM>>(Request.Cookies["Basket"]);

                    foreach (var cookie in cookies)
                    {
                        Product product = await _context.Products
                            .Include(p => p.ProductImages.Where(pi => pi.IsPrimary == true))
                            .FirstOrDefaultAsync(p => p.Id == cookie.Id);

                        if (product is not null)
                        {
                            BasketItemVM item = new BasketItemVM
                            {
                                Id = product.Id,
                                Name = product.Name,
                                Image = product.ProductImages.FirstOrDefault().Url,
                                Price = product.Price,
                                Count = cookie.Count,
                                SubTotal = product.Price * cookie.Count
                            };
                            items.Add(item);
                        }
                    }
                }

            }

            return View(items);
        }
        public async Task<IActionResult> AddBasket(int id)
        {
            if (id <= 0) return BadRequest();
            Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (product is null) return NotFound();
            List<BasketCookieItemVM> basket;

            if (User.Identity.IsAuthenticated)
            {
                
                AppUser? user = await _userManager.Users
                    .Include(u => u.BasketItems.Where(bi => bi.OrderId == null))
                    .FirstOrDefaultAsync(u => u.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));
                if (user == null) return NotFound();
                BasketItem item = user.BasketItems.FirstOrDefault(bi => bi.ProductId == product.Id);
                if (item is null)
                {
                    item = new BasketItem
                    {
                        AppUserId = user.Id,
                        ProductId = product.Id,
                        Count = 1,
                        Price = product.Price,


                    };
                    user.BasketItems.Add(item);
                }
                else
                {
                    item.Count++;
                }

                await _context.SaveChangesAsync();
            }
            else
            {

                if (Request.Cookies["Basket"] is null)
                {
                    basket = new List<BasketCookieItemVM>();
                    BasketCookieItemVM item = new BasketCookieItemVM
                    {
                        Id = id,
                        Count = 1
                    };
                    basket.Add(item);
                }
                else
                {
                    basket = JsonConvert.DeserializeObject<List<BasketCookieItemVM>>(Request.Cookies["Basket"]);

                    BasketCookieItemVM existed = basket.FirstOrDefault(b => b.Id == id);

                    if (existed is null)
                    {
                        BasketCookieItemVM item = new BasketCookieItemVM
                        {
                            Id = id,
                            Count = 1
                        };
                        basket.Add(item);
                    }
                    else
                    {
                        existed.Count++;
                    }
                }



                string json = JsonConvert.SerializeObject(basket);

                Response.Cookies.Append("Basket", json);
            }



            return RedirectToAction(nameof(Index), "Home");


        }

        public async Task<IActionResult> Chechout()
        {
            AppUser user = await _userManager.Users
                .Include(u => u.BasketItems.Where(bi => bi.OrderId == null))
                .ThenInclude(bi => bi.Product)
                .FirstOrDefaultAsync(u => u.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));

            OrderVM orderVM = new OrderVM
            {
                BasketItems = user.BasketItems
            };
            return View(orderVM);
        }

        public async Task<IActionResult> Chechout(OrderVM orderVM)
        {
            AppUser user = await _userManager.Users


               .Include(u => u.BasketItems.Where(bi => bi.OrderId == null))

               .ThenInclude(bi => bi.Product)

               .FirstOrDefaultAsync(u => u.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (!ModelState.IsValid)
            {
                orderVM.BasketItems = user.BasketItems;
                return View(orderVM);
            }

            decimal total = 0;

            foreach (var item in user.BasketItems)
            {
                item.Price = item.Product.Price;
                total += item.Price * item.Price;
            }


            Order order = new Order
            {

                Status = null,
                Adress = orderVM.Address,
                AppUserId = user.Id,
                PurchasedAt = DateTime.Now,
                BasketItems = user.BasketItems,
                TotalPrice = total
            };

            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
            string body = @"
                           Your order successfully placed:
                           <table border=""1"">                       
                              <thead>
                                 <tr>
                                     <th>Name</th>
                                     <th>Price</th>        
                                     <th>Count</th>    
                                 </tr>
                             </thead>
                           <tbody>";
            foreach (var item in order.BasketItems)
            {
                body += @$" <tr>
                                     <td>{item.Product.Name}</td>
                                     <td>{item.Price}</td>
                                     <td>{item.Count}</td>
                          </tr>";
            }
            body += @"<tbody>
                     </table>";



            await _emailService.SendEmailAysnc(user.Email, "Your Order", body, true);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult GetBasket()
        {
            return Content(Request.Cookies["Basket"]);
        }

    }
}
