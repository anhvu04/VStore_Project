using MediatR;
using Microsoft.AspNetCore.Mvc;
using VStore.Application.Usecases.Voucher.Command.CreateVoucher;
using VStore.Application.Usecases.Voucher.Command.DeleteVoucher;
using VStore.Application.Usecases.Voucher.Command.UpdateVoucher;
using VStore.Application.Usecases.Voucher.Query.GetVoucher;
using VStore.Application.Usecases.Voucher.Query.GetVouchers;

namespace VStore.API.Controllers;

[Route("api/vouchers")]
public class VoucherController(ISender sender) : ApiController(sender)
{
    [HttpGet]
    public async Task<IActionResult> GetVouchers([FromQuery] GetVouchersQuery query)
    {
        var result = await Sender.Send(query);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetVoucher(Guid id)
    {
        var result = await Sender.Send(new GetVoucherQuery(id));
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPost]
    public async Task<IActionResult> CreateVoucher([FromBody] CreateVoucherCommand command)
    {
        var result = await Sender.Send(command);
        return result.IsSuccess ? Ok() : BadRequest(result.Error);
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateVoucher(Guid id, [FromBody] UpdateVoucherCommand command)
    {
        command.Id = id;
        var result = await Sender.Send(command);
        return result.IsSuccess ? Ok() : BadRequest(result.Error);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteVoucher(Guid id)
    {
        var result = await Sender.Send(new DeleteVoucherCommand(id));
        return result.IsSuccess ? Ok() : BadRequest(result.Error);
    }
}