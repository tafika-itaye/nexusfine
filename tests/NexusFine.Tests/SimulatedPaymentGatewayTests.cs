using Microsoft.Extensions.Logging.Abstractions;
using NexusFine.Infrastructure.Services;
using Xunit;

namespace NexusFine.Tests;

public class SimulatedPaymentGatewayTests
{
    private SimulatedPaymentGateway NewGateway() =>
        new(NullLogger<SimulatedPaymentGateway>.Instance);

    [Fact]
    public async Task Initiate_ReturnsSuccessWithSimReference()
    {
        var gw  = NewGateway();
        var res = await gw.InitiateAsync(new GatewayRequest
        {
            Phone     = "+265991000001",
            Amount    = 50000m,
            Reference = "NXF-2026-00001"
        });

        Assert.True(res.Success);
        Assert.StartsWith("SIM-", res.TransactionReference);
        Assert.Equal("Pending", res.Status);
    }

    [Fact]
    public async Task QueryStatus_ReturnsSuccess()
    {
        var gw  = NewGateway();
        var res = await gw.QueryStatusAsync("SIM-xyz");
        Assert.Equal("Success", res.Status);
    }

    [Fact]
    public async Task HandleCallback_ConfirmedPayload_IsConfirmed()
    {
        var gw  = NewGateway();
        var res = await gw.HandleCallbackAsync(
            "{\"reference\":\"SIM-abc\",\"amount\":50000,\"confirmed\":true}");

        Assert.True(res.IsPaymentConfirmed);
        Assert.Equal("SIM-abc", res.TransactionReference);
        Assert.Equal(50000m, res.Amount);
    }

    [Fact]
    public async Task HandleCallback_NotConfirmed_IsFalse()
    {
        var gw  = NewGateway();
        var res = await gw.HandleCallbackAsync(
            "{\"reference\":\"SIM-abc\",\"amount\":50000,\"confirmed\":false}");

        Assert.False(res.IsPaymentConfirmed);
    }
}
