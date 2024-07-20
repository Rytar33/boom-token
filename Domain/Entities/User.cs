using Domain.Primitives.Enums;
using Domain.Extensions;

namespace Domain.Entities;

public class User : BaseEntity
{
    public User(long telegramId) 
    {
        TelegramId = telegramId;
        Balance = 0;
        LimitEnergy = 500;
        Energy = 500;
        EnergyRecoveryInSecond = 1;
        RewardForClick = 1;
        CountClick = 0;
        DateTimeRegistration = DateTime.Now;
        RangTap = RangTap.White;
        ReferalLink = StringExtension.GenerateReferralCode();
    }

    public long TelegramId { get; private set; }

    public long Balance { get; private set; }

    public RangTap RangTap { get; private set; }

    public int LimitEnergy { get; private set; }

    public int Energy { get; private set; }

    public int EnergyRecoveryInSecond { get; private set; }

    public int RewardForClick { get; private set; }

    public long CountClick { get; private set; }

    public string ReferalLink { get; }

    public DateTime DateTimeRegistration { get; }

    public void Update(
        long? telegramId = null,
        long? balance = null,
        int? limitEnergy = null,
        int? energy = null,
        int? energyRecoveryInSecond = null,
        long? countClick = null,
        int? rewardForClick = null)
    {
        if (telegramId != null)
            TelegramId = telegramId.Value;
        if (balance != null)
        {
            Balance = balance.Value;
            RangTap = Balance switch
            {
                >= (long)RangTap.Briliant => RangTap.Briliant,
                >= (long)RangTap.Platinum => RangTap.Platinum,
                >= (long)RangTap.Gold => RangTap.Gold,
                >= (long)RangTap.Silver => RangTap.Silver,
                >= (long)RangTap.Bronza => RangTap.Bronza,
                _ => RangTap.White
            };
        }
        if (limitEnergy != null)
            LimitEnergy = limitEnergy.Value;
        if (energy != null)
            Energy = energy.Value;
        if (energyRecoveryInSecond != null)
            EnergyRecoveryInSecond = energyRecoveryInSecond.Value;
        if (countClick != null)
            CountClick = countClick.Value;
        if (rewardForClick != null)
            RewardForClick = rewardForClick.Value;
    }
}
