using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VStore.API.Common;
using VStore.Application.Usecases.Voucher.Command.CreateVoucher;
using VStore.Application.Usecases.Voucher.Command.DeleteVoucher;
using VStore.Application.Usecases.Voucher.Command.UpdateVoucher;
using VStore.Application.Usecases.Voucher.Query.GetVoucher;
using VStore.Application.Usecases.Voucher.Query.GetVouchers;
using VStore.Domain.AuthenticationScheme;
using VStore.Domain.Enums;

namespace VStore.API.Controllers;

[Route("api/vouchers")]
public class VoucherController(ISender sender) : ApiController(sender)
{
    [HttpGet]
    [CacheResponseAttribute(600)]
    public async Task<IActionResult> GetVouchers([FromQuery] GetVouchersQuery query)
    {
        var result = await Sender.Send(query);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpGet("{id}")]
    [CacheResponseAttribute(600)]
    public async Task<IActionResult> GetVoucher(Guid id)
    {
        var result = await Sender.Send(new GetVoucherQuery(id));
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = AuthenticationScheme.Access, Roles = nameof(Role.Admin))]
    [InvalidateCache("/api/vouchers/")]
    public async Task<IActionResult> CreateVoucher([FromBody] CreateVoucherCommand command)
    {
        var result = await Sender.Send(command);
        return result.IsSuccess ? Ok() : BadRequest(result.Error);
    }

    [HttpPatch("{id}")]
    [Authorize(AuthenticationSchemes = AuthenticationScheme.Access, Roles = nameof(Role.Admin))]
    [InvalidateCache("/api/vouchers/")]
    public async Task<IActionResult> UpdateVoucher(Guid id, [FromBody] UpdateVoucherCommand command)
    {
        command.Id = id;
        var result = await Sender.Send(command);
        return result.IsSuccess ? Ok() : BadRequest(result.Error);
    }

    [HttpDelete("{id}")]
    [Authorize(AuthenticationSchemes = AuthenticationScheme.Access, Roles = nameof(Role.Admin))]
    [InvalidateCache("/api/vouchers/")]
    public async Task<IActionResult> DeleteVoucher(Guid id)
    {
        var result = await Sender.Send(new DeleteVoucherCommand(id));
        return result.IsSuccess ? Ok() : BadRequest(result.Error);
    }
}