const EOrderPayType = Object.freeze({
    TOTAL: 0,
    SHIP: 1,
    PROMOTION: 2,
    OTHER1: 3,
    OTHER2: 4,
    OTHER3: 5,
    OTHER4: 6,
    OTHER5: 7,
    OTHER6: 8,
    OTHER7: 9,
    FINAL: 10
});

const EOrderStatus = Object.freeze(
    {
        PROCESSING: 0,
        SHIPPING: 1,
        RECEIVED: 2,
        CANCELLED: 3,
        RETURNED: 4,
        COMPLETED: 5
    });

const EPaymentMethod = Object.freeze(
    {
        CASH_ON_DELIVERY: 0,
        BANK_TRANSFER: 1
    }
);

const EOrderFrom = Object.freeze(
    {
        VOI_BE_NHO: 0,
        FACEBOOK: 1,
        TIKTOK: 2,
        OTHER: 3
    }
);

const EOrderPayStatus = Object.freeze(
    {
        PENDING: 0,
        PAID: 1,
        REFUNDED: 2
    });

const EOrderSimplePromotionType = Object.freeze(
    {
        /// <summary>
        /// 0
        /// </summary>
        SHIP_DISCOUNT: 0,

        /// <summary>
        /// 1
        /// </summary>
        TOTAL_DISCOUNT: 1
    });

const EOrderFilterStatus = Object.freeze({
    PROCESSING: 0,
    SHIPPING: 1,
    RECEIVED: 2,
    CANCELLED: 3,
    RETURNED: 4,
    COMPLETED: 5,
    PENDING: 6,
    PAID: 7,
    REFUNDED: 8,
    ALL: 10
});
