namespace MVCPlayWithMe.OpenPlatform.Model
{
    public enum EnumCommerceTypeDeTail
    {
        TIKI,
        SHOPEE_ITEM,
        SHOPEE_MODEL,
        NONE
    }

    /// <summary>
    /// Lớp base đơn hàng
    /// </summary>
    public class GeneralOrder
    {
        public EnumCommerceTypeDeTail objectType { get;}

        public GeneralOrder (EnumCommerceTypeDeTail type)
        {
            objectType = type;
            shipCode = string.Empty;
        }

        /// <summary>
        /// Mã vận đơn
        /// Shopee sinh mã vận đơn sau khi tạo bill,
        /// trên bill chỉ sinh barcode, QR code cho mã vận đơn không sinh cho mã đơn
        /// </summary>
        public string shipCode { get; set; }

        public string TypeToString()
        {
            if (objectType == EnumCommerceTypeDeTail.TIKI)
                return "Tiki";
            else if (objectType == EnumCommerceTypeDeTail.SHOPEE_ITEM || 
                objectType == EnumCommerceTypeDeTail.SHOPEE_MODEL)
                return "Shopee";
            return string.Empty;
        }
    }
}
