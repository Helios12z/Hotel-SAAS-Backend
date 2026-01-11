namespace Hotel_SAAS_Backend.API.Models.Enums
{
    public enum PromotionType
    {
        Percentage,      // Gi?m theo %
        FixedAmount,     // Gi?m s? ti?n c? ??nh
        FreeNight,       // T?ng ?êm mi?n phí (book 3 ?êm t?ng 1)
        EarlyBird,       // ??t s?m gi?m giá
        LastMinute       // ??t phút chót gi?m giá
    }

    public enum PromotionStatus
    {
        Draft,           // ?ang so?n
        Active,          // ?ang ho?t ??ng
        Paused,          // T?m d?ng
        Expired,         // H?t h?n
        Cancelled        // ?ã h?y
    }

    public enum CouponStatus
    {
        Active,
        Used,
        Expired,
        Cancelled
    }
}
