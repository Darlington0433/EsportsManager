using System;
namespace EsportsManager.BL.Models;

public class Wallet
{
    public int WalletId { get; set; }
    public int UserId { get; set; }
    public decimal Balance { get; set; }
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
}
