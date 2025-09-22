using AppView.Clients.ApiClients;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppView.Controllers
{
	[Authorize]
	public class ShippingAddressController : Controller
	{
		private readonly IShippingAddressClient _client;
		public ShippingAddressController(IShippingAddressClient client)
		{
			_client = client;
		}

		[HttpGet]
		public async Task<IActionResult> Index()
		{
			var list = await _client.GetMyAddressesAsync();
			return View(list);
		}

		[HttpGet]
		public async Task<IActionResult> Get(Guid id)
		{
			var item = await _client.GetAsync(id);
			if (item == null) return NotFound();
			return Json(item);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([FromForm] ShippingAddressCreateVm vm)
		{
			if (!ModelState.IsValid) return BadRequest(ModelState);
			var result = await _client.CreateAsync(vm);
			if (!result.Success)
			{
				return StatusCode(result.StatusCode, result.Error ?? "Create failed");
			}
			return Json(result.Data);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Update(Guid id, [FromForm] ShippingAddressCreateVm vm)
		{
			if (!ModelState.IsValid) return BadRequest(ModelState);
			var updated = await _client.UpdateAsync(id, vm);
			if (updated == null) return NotFound();
			return Json(updated);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Delete(Guid id)
		{
			var ok = await _client.DeleteAsync(id);
			if (!ok) return NotFound();
			return Ok();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> SetDefault(Guid id)
		{
			var ok = await _client.SetDefaultAsync(id);
			if (!ok) return NotFound();
			return Ok();
		}
	}
}

