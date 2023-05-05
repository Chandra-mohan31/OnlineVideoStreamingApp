using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineVideoStreamingApp.Models;
using Razorpay.Api;

namespace OnlineVideoStreamingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RazorPayController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public RazorPayController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        [HttpGet]
        public async Task<string> GenerateOrderId()
        {

            //await order id an return it
            string apiUrl = "https://api.razorpay.com/v1/orders";
            string orderId = "";
            try
            {
              

                RazorpayClient client = new RazorpayClient(_configuration["razor_pay_access_key"], _configuration["razor_pay_secret_key"]);

            var orderOptions = new Dictionary<string, object>
            {
                { "amount", 100 },
                { "currency", "INR" },
                { "receipt", "order_recptid_11" },
                { "payment_capture", 1 }
            };
                var order = client.Order.Create(orderOptions);
                orderId = order["id"].ToString();
                Console.WriteLine(orderId);
            }
            catch(Exception ex) 
            {
                Console.WriteLine(ex.Message);
            }
            return orderId;
        }
    }
}
