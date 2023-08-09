using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Web;

namespace MVCPlayWithMe.General
{
    public enum ParameterSearch
    {
        First,
        Last,
        All,
        Same
    }

    public enum EnumCommerceTypeDeTail
    {
        TIKI,
        SHOPEE_ITEM,
        SHOPEE_MODEL,
        NONE
    }

    public enum EnumCommerceType
    {
        ALL,
        TIKI,
        SHOPEE,
    }

    class CommonQLK
    {
        /// <summary>
        /// Kiểm tra 1 string có là số nguyên hợp lệ
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        /// 
        public const Int32 maxInteger = 10000;

        /// <summary>
        /// Phát âm thanh báo có lỗi
        /// </summary>
        public static void PlayErrorSound()
        {
            SystemSounds.Asterisk.Play();
        }

        ///// <summary>
        ///// Phát âm thanh báo có lỗi
        ///// </summary>
        //public static void PlayErrorSoundAndMess(string mess)
        //{
        //    SystemSounds.Asterisk.Play();
        //    MessageBox.Show(mess, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
        //}

        ///// <summary>
        ///// Phát âm thanh báo có lỗi
        ///// </summary>
        //public static MessageBoxResult PlayErrorSoundAndMessAndAskContinue(string mess)
        //{
        //    SystemSounds.Asterisk.Play();
        //    return MessageBox.Show(mess, "Lỗi", MessageBoxButton.YesNo, MessageBoxImage.Error);
        //}

        ///// <summary>
        ///// Phát âm thanh 
        ///// </summary>
        //public static void PlayOkMess(string mess)
        //{
        //    MessageBox.Show(mess, "", MessageBoxButton.OK, MessageBoxImage.None);
        //}

        /// <summary>
        /// Thời gian để 1 message box tự đóng tính theo miliseconds
        /// </summary>
        public static Int32 timeToCloseMessageBox = 2000;

        public static Boolean IsInteger(string text)
        {
            if (String.IsNullOrEmpty(text))
                return false;

            // Check cho phép 1 ký tự dấu âm '-' ở đầu string
            if (text.Length == 1 && text.ElementAt(0) == '-')
                return false;

            int l = text.Length;

            for (int i = 1; i < l; i++)
            {
                if (!Char.IsDigit(text.ElementAt(i)))
                    return false;
            }

            try
            {
                int result = Int32.Parse(text);
                if (result > maxInteger)
                {
                    return false;
                }
                Console.WriteLine(result);
            }
            catch (FormatException)
            {
                //Console.WriteLine($"Unable to parse '{input}'");
                MyLogger.GetInstance().DebugFormat("Unable to parse '{text}'", text);
                return false;
            }
            return true;
        }

        public const string commerceNameTiki = "Tiki";
        public const string commerceNameShopee = "Shopee";

        public const string outputOrder = "Đã Đóng";
        public const string returnOrder = "Đã Hoàn";

        public const string turnOffProductTiki = "Đang Tắt";
        public const string turnOnProductTiki = "Đang Bật";
        public const int turnOffProductTikiPara = 0;
        public const int turnOnProductTikiPara = 1;

        public const string HidenProductTiki = "Đang Ẩn";
        public const string VisbleOnProductTiki = "Đang Hiện";

        public const string NormalProductShopee = "NORMAL";
        public const string BannedProductShopee = "BANNED";
        public const string DeletedProductShopee = "DELETED";
        public const string UnlistProductShopee = "UNLIST";

        public const string UpdateQuantitySucces = "Xong";

        public const string productStatusOn = "On";// Tương ứng Đang Bật trên Tiki và NORMAL trên Shopee
        public const string productStatusOff = "Off";// Tương ứng Đang Tắt" trên Tiki và DELETED/BANNED trên Shopee

        public const string productMapping = "Mapped";

        // Trạng thái sản phẩm trong kho. On/Off tương ứng kinh doanh hoặc ngừng kinh doanh
        public const string productStatusInWarehouseOn = "On";
        public const string productStatusInWarehouseOff = "Off";

        public const string publisherNameAll = "Tất cả nhà phát hành";


        public static Int32 ConvertStringToInt32(string str)
        {
            if (String.IsNullOrEmpty(str))
            {
                return 0;
            }
            else
            {
                try
                {
                    return Int32.Parse(str);
                }
                catch (FormatException ex)
                {
                    throw new FormatException("Không thể chuyển đổi sang số do sai định dạng. " + ex.Message);
                }
                catch (OverflowException ex)
                {
                    throw new OverflowException("Giá trị số quá lớn. " + ex.Message);
                }
            }
        }

        public static long ConvertStringToInt64(string str)
        {
            if (String.IsNullOrEmpty(str))
            {
                return 0;
            }
            else
            {
                try
                {
                    return Int64.Parse(str);
                }
                catch (FormatException ex)
                {
                    throw new FormatException("Không thể chuyển đổi sang số do sai định dạng. " + ex.Message);
                }
                catch (OverflowException ex)
                {
                    throw new OverflowException("Giá trị số quá lớn. " + ex.Message);
                }
            }
        }

        public static string ConvertIntToVNDFormat(int money)
        {
            // Thêm ','
            StringBuilder sb = new StringBuilder();
            sb.Append(money.ToString());
            int length = sb.Length;
            if (length > 9)
            {
                sb.Insert(length - 3, ',');
                sb.Insert(length - 6, ',');
                sb.Insert(length - 9, ',');

            }
            else if (length > 6)
            {
                sb.Insert(length - 3, ',');
                sb.Insert(length - 6, ',');

            }
            else if (length > 3)
            {
                sb.Insert(length - 3, ',');
            }
            return sb.ToString();
        }

        public static int ConverVNDFormatToInt(string str)
        {
            return ConvertStringToInt32(str.Replace(",", ""));
        }

        public static Boolean CheckValidDiscount(string str)
        {
            if (str.ElementAt(str.Length - 1) != '%')
                return false;

            string strTem = str.Remove(str.Length - 1);
            int n;
            bool isNumeric = int.TryParse(strTem, out n);

            if (!isNumeric)
                return false;

            if (n > 100 || n < 0)
                return false;
            return true;
        }

        public static Boolean CheckValidOpacity(string str)
        {
            float number = 0;
            if (!float.TryParse(str, out number))
                return false;
            if (number < 0 || number > 1)
                return false;
            return true;
        }

        /// <summary>
        /// Hàm trả về fail, chi tiết lỗi sẽ được lưu trong biến này
        /// </summary>
        public static string CommonErrorMessage;

        #region Message box tự động đóng sau n giây
        //[DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true, CharSet = CharSet.Auto)]
        //static extern IntPtr FindWindowByCaption(IntPtr ZeroOnly, string lpWindowName);

        //[DllImport("user32.Dll")]
        //static extern int PostMessage(IntPtr hWnd, UInt32 msg, int wParam, int lParam);

        //private const UInt32 WM_CLOSE = 0x0010;

        //public static void ShowAutoClosingMessageBox(string message, string caption)
        //{
        //    var timer = new System.Timers.Timer(timeToCloseMessageBox) { AutoReset = false };
        //    timer.Elapsed += delegate
        //    {
        //        IntPtr hWnd = FindWindowByCaption(IntPtr.Zero, caption);
        //        if (hWnd.ToInt32() != 0)
        //            PostMessage(hWnd, WM_CLOSE, 0, 0);
        //    };
        //    timer.Enabled = true;
        //    MessageBox.Show(message, caption, MessageBoxButton.OK, MessageBoxImage.None);
        //}
        #endregion

        #region Thời gian
        /// <summary>
        /// Định dạng: YYYY
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static Boolean CheckYear(string text)
        {
            Int32 year;
            try
            {
                year = Int32.Parse(text);
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.Message);
                return false;
            }
            if (year > 9999 || year < 1900)
                return false;

            return true;
        }

        /// <summary>
        /// Định dạng: M hoặc MM
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static Boolean CheckMonth(string text)
        {
            Int32 month;
            try
            {
                month = Int32.Parse(text);
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.Message);
                return false;
            }
            if (month > 12 || month < 1)
                return false;

            return true;
        }

        /// <summary>
        /// Định dạng D hoặc DD
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static Boolean CheckDayOfMonth(string text)
        {
            Int32 day;
            try
            {
                day = Int32.Parse(text);
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.Message);
                return false;
            }
            if (day > 31 || day < 1)
                return false;

            return true;
        }

        /// <summary>
        ///  Check text có thể convert sang dạng thời gian
        ///  03/08/1989 -> DD/MM/YYYY
        ///  3/8/1989 -> DD/MM/YYYY
        ///  8/1989 -> MM/YYYY
        ///  1989->YYYY
        /// </summary>
        /// <param name="text"></param>
        /// <param name="enableNullOrEmpty">True: Cho phép text là null hay empty</param>
        /// <returns></returns>
        public static Boolean CheckTimeValid(string text, Boolean enableNullOrEmpty)
        {
            if (string.IsNullOrEmpty(text))
            {
                if (enableNullOrEmpty)
                    return true;
                else
                    return false;
            }

            char[] delimiterChars = { '_', '.', '-', '/' };
            string[] words = text.Split(delimiterChars);
            Boolean isOk = true;
            do
            {
                int length = words.Length;
                if (length > 3 || length < 1)
                {
                    isOk = false;
                    break;
                }

                // Text dạng YYYY
                if (length == 1)
                {
                    if (!CheckYear(words[0]))
                    {
                        isOk = false;
                        break;
                    }
                }
                else if (length == 2) // Text dạng MM/YYYY
                {
                    if (!CheckMonth(words[0]) || !CheckYear(words[1]))
                    {
                        isOk = false;
                        break;
                    }
                }
                else // Text dạng DD/MM/YYYY
                {
                    if (!CheckDayOfMonth(words[0]) || !CheckMonth(words[1]) || !CheckYear(words[2]))
                    {
                        try
                        {
                            DateTime dt = DateTime.ParseExact(text, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        }
                        catch (Exception ex)
                        {
                            string ms = ex.Message;
                            isOk = false;
                        }
                    }
                }

            } while (false);

            if (!isOk)
            {
                return false;
            }
            return true;
        }

        public static long GetTimestampNow()
        {
            DateTime start = DateTime.Now;
            long timest = ((DateTimeOffset)start).ToUnixTimeSeconds();
            return timest;
        }

        public static long GetTimestamp(DateTime dt)
        {
            long timest = ((DateTimeOffset)dt).ToUnixTimeSeconds();
            return timest;
        }

        public static DateTime GetDateFromTimestamp(long timest)
        {
            DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(timest);
            DateTime dateTime = dateTimeOffset.LocalDateTime;
            return dateTime;
        }

        public static string GetTimeNowddMMYYYY()
        {
            return DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
        }

        public static DateTime ConvertStringToDateTime(string str)
        {
            // Trả giá trị mặc định là ngày sinh Sâu béo
            if (string.IsNullOrEmpty(str))
                return DateTime.ParseExact("05/08/2018 01:01:01", "dd/MM/yyyy", CultureInfo.InvariantCulture);

            return DateTime.ParseExact(str, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
        }
        #endregion

        /// <summary>
        /// Check xem ảnh đã tồn tại trong thư mục hay chưa? Nếu chưa ải ảnh từ địa chỉ web và lưu
        /// </summary>
        /// <param name="url">https://salt.tikicdn.com/cache/280x280/ts/product/c5/53/ad/991011e797c67d6910b87491ddeee138.png</param>
        ///                   https://cf.shopee.vn/file/673f310b9b9152f0898752eb56e67ac6_tn
        /// <param name="pathFolder">Thư mục chứa ảnh</param>
        public static void DownloadImageAndSave(string url, string pathFolder)
        {
            //// Từ url lấy được tên ảnh
            //string fileName = GetNameFromURL(url);
            //if (string.IsNullOrEmpty(fileName))
            //    return;

            //// Check xem ảnh đã tồn tại hay chưa?
            //if (File.Exists(Path.Combine(pathFolder, fileName)))
            //    return;

            //RestClient client = new RestClient(url);
            //client.Timeout = -1;
            //RestRequest request = new RestRequest(Method.GET);
            ////IRestResponse response = client.Execute(request);

            //try
            //{
            //    var fileBytes = client.DownloadData(request);
            //    File.WriteAllBytes(Path.Combine(pathFolder, fileName), fileBytes);
            //}
            //catch (Exception ex)
            //{
            //    MyLogger.GetInstance().Warn(ex.ToString());
            //}
        }

        /// <summary>
        ///  Từ url lấy được tên file
        /// </summary>
        /// <param name="url"https://salt.tikicdn.com/cache/280x280/ts/product/c5/53/ad/991011e797c67d6910b87491ddeee138.png </param>
        ///                  https://cf.shopee.vn/file/673f310b9b9152f0898752eb56e67ac6_tn
        /// <returns></returns>
        public static String GetNameFromURL(string url)
        {
            string fileName = string.Empty;
            if (string.IsNullOrEmpty(url))
                return fileName;

            // Lấy tên file ảnh
            // Từ url lấy được tên ảnh
            int lastIndex = url.LastIndexOf('/');
            if (lastIndex == -1 || lastIndex == url.Length - 1)
            {
            }
            else
            {
                fileName = url.Substring(lastIndex + 1);
                //name = Path.Combine(((App)Application.Current).temporaryTikiImageFolderPath, fileName);
            }
            if (!string.IsNullOrEmpty(fileName))
            {
                string extention = Path.GetExtension(fileName);
                if (string.IsNullOrEmpty(extention))
                    fileName = fileName + ".jfif";
            }
            return fileName;
        }

        /// <summary>
        /// Lấy được đường dẫn đầy đủ của ảnh tải từ sàn TMDT
        /// </summary>
        /// <param name="url"></param>
        /// <param name="pathFolder"></param>
        public static string GetFullPathOfImage(string url, string pathFolder)
        {
            //if (string.IsNullOrEmpty(url) || string.IsNullOrEmpty(pathFolder))
            //    return string.Empty;

            //string fileName = GetNameFromURL(url);

            //if (string.IsNullOrEmpty(fileName))
            //    return string.Empty;

            //if (!File.Exists(Path.Combine(pathFolder, fileName)))
            //{
            //    // Nếu ảnh chưa có ta download ảnh
            //    RestClient client = new RestClient(url);
            //    client.Timeout = -1;
            //    RestRequest request = new RestRequest(Method.GET);

            //    try
            //    {
            //        var fileBytes = client.DownloadData(request);
            //        File.WriteAllBytes(Path.Combine(pathFolder, fileName), fileBytes);
            //    }
            //    catch (Exception ex)
            //    {
            //        MyLogger.GetInstance().Warn(ex.ToString());
            //        return string.Empty;
            //    }
            //}

            //return Path.Combine(pathFolder, fileName);
            return string.Empty;
        }

        /// <summary>
        /// Mã sản phẩm mẹ chứa hoặc bằng mã sản phẩm con
        /// Vì mã sản phẩm mẹ có dạng 8938532871251-8938532871237-8938532871411 nên 1 sản phẩm con có mã 8938532871251, hoặc 8938532871237 hoặc 8938532871251-8938532871237 đều là chỉ sản phẩm mẹ
        /// </summary>
        /// <param name="motherMaSP"></param>
        /// <param name="childMaSP"></param>
        /// <returns>true: 2 mã là của 1 sản phẩm ngược lại false</returns>
        public static bool CheckMotherAndChildCode(string motherMaSP, string childMaSP)
        {
            if (string.IsNullOrEmpty(motherMaSP) || string.IsNullOrEmpty(childMaSP))
                return false;
            //Boolean add1 = false;
            //Boolean add2 = false;
            string[] mothers = motherMaSP.Split('-');
            string[] children = childMaSP.Split('-');

            for (int i = 0; i < children.Length; i++)
            {
                for (int j = 0; j < mothers.Length; j++)
                {
                    if (children[i].Equals(mothers[j]))
                    {
                        //add1 = true;
                        //break;
                        return true;
                    }
                }
                //if (add1 == true)
                //    break;
            }
            //if (add1 && motherMaSP.Contains(childMaSP))
            //{
            //    add2 = true;
            //}

            //return (add1 && add2);
            return false;
        }

        /// <summary>
        /// VD: Chúc Bé Ngủ Ngon - Combo Black And White Books
        /// Tên sản phẩm như trên thì Combo Black And White Books là tên combo
        /// </summary>
        /// <param name="productName"></param>
        /// <returns></returns>
        public static string GetComboNameFromProductName(string productName)
        {
            string[] names = productName.Split('-');
            if (names.Count() < 1)
                return string.Empty;

            string comboName = names[names.Count() - 1];
            if (string.IsNullOrEmpty(comboName) || string.IsNullOrEmpty(comboName.Trim()))
                return string.Empty;

            return comboName.Trim();
        }

        public static void GetAllMediaFiles(List<string> listMediaFiles, string folderPath)
        {
            //listMediaFiles.Clear();

            //// Check thư mục chứa tồn tại
            //if (String.IsNullOrEmpty(folderPath))
            //    return;
            //if (!Directory.Exists(folderPath))
            //    return;

            //// Image formats
            //List<string> listTemp = ((App)Application.Current).GetListImageFormats();
            //if (listTemp != null && listTemp.Count() != 0)
            //{
            //    foreach (string str in listTemp)
            //    {
            //        listMediaFiles.AddRange(Directory.GetFiles(folderPath, str).ToList());
            //    }
            //}
            //// Video formats
            //listTemp = ((App)Application.Current).GetListVideoFormats();
            //if (listTemp != null && listTemp.Count() != 0)
            //{
            //    foreach (string str in listTemp)
            //    {
            //        listMediaFiles.AddRange(Directory.GetFiles(folderPath, str).ToList());
            //    }
            //}
            //listMediaFiles.Sort();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName">Gồm đường dẫn</param>
        /// <returns></returns>
        public static Boolean IsImageFormat(string fileName)
        {
            //string extension = Path.GetExtension(fileName);
            //// Image formats
            //List<string> listTemp = ((App)Application.Current).GetListImageFormats();
            //foreach (var ex in listTemp)
            //{
            //    if (ex.Contains(extension))
            //        return true;
            //}
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName">Gồm đường dẫn</param>
        /// <returns></returns>
        public static Boolean IsVideoFormat(string fileName)
        {
            //string extension = Path.GetExtension(fileName);
            //// Video formats
            //List<string> listTemp = ((App)Application.Current).GetListVideoFormats();
            //foreach (var ex in listTemp)
            //{
            //    if (ex.Contains(extension))
            //        return true;
            //}
            return false;
        }

        //public static BitmapImage GetBitmapImage(string PathToImage)
        //{
        //    BitmapImage bi = new BitmapImage();
        //    bi.BeginInit();
        //    bi.CacheOption = BitmapCacheOption.OnLoad;
        //    bi.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
        //    bi.UriSource = new Uri(PathToImage);
        //    bi.EndInit();
        //    return bi;
        //}

        public static string GetPriceAfterDiscount(int listedPrice, string discount)
        {
            string str = string.Empty;
            return str;
        }

        /// <summary>
        ///  Check giá tiền cụ thể hay % chiết khấu
        /// </summary>
        /// <param name="textDiscount">Giá bán thực tế hoặc % chiết khấu với '%' sau cùng</param>
        /// <param name="marketPrice">Giá bìa</param>
        /// <param name="price">giá thực tế bán</param>
        /// <param name="currentPrice">giá đang bán</param>
        /// <returns></returns>
        public static string CheckDiscountAndGetPrice(string textDiscount, int marketPrice, ref int price, int currentPrice)
        {
            string str = string.Empty;
            bool isInvalid;
            if (textDiscount.ElementAt(textDiscount.Length - 1) == '%')
            {
                int discount;
                isInvalid = int.TryParse(textDiscount.Remove(textDiscount.Length - 1), out discount);
                if (!isInvalid || discount > 100 || discount < 0)
                {
                    str = "Chiết khấu không chính xác.";
                }
                price = marketPrice * (100 - discount) / 100;
            }
            else
            {
                isInvalid = int.TryParse(textDiscount, out price);
                if (!isInvalid || price > marketPrice || price < 0)
                {
                    str = "Giá không chính xác.";
                }
            }
            if (string.IsNullOrEmpty(str))
            {
                price = price / 1000 * 1000; // Lấy đơn vị tròn 1000 vnđ
                if (price == currentPrice)
                {
                    str = "Bằng giá đang bán. Không cần xét lại.";
                }
            }
            return str;
        }
    }

    /// <summary>
    /// Thực hiện tìm kiếm theo text
    /// </summary>
    //class SearchInListString
    //{
    //    string strOne; // Tham số tìm kiếm 1 ký tự
    //    string strTwo; // Tham số tìm kiếm 2 ký tự
    //    ObservableCollection<string> lsOne; // Kết quả khi tham số tìm kiếm dài 1 ký tự
    //    ObservableCollection<string> lsTwo; // Kết quả khi tham số tìm kiếm dài 2 ký tự
    //    ObservableCollection<string> listOriginal; // Danh sách string ban đầu, không thay đổi
    //    ParameterSearch parameterSearch;

    //    public SearchInListString(ObservableCollection<string> lsInput, ParameterSearch paraInput)
    //    {
    //        strOne = string.Empty;
    //        strTwo = string.Empty;
    //        lsOne = null;
    //        lsTwo = null;
    //        listOriginal = lsInput;
    //        parameterSearch = paraInput;
    //    }

    //    private ObservableCollection<string> SearchPure(ObservableCollection<string> lsInput, string paraStr)
    //    {
    //        ObservableCollection<string> list = new ObservableCollection<string>();
    //        Int32 count = lsInput.Count();
    //        if (parameterSearch == ParameterSearch.First)
    //        {
    //            for (Int32 i = 0; i < count; i++)
    //            {
    //                if (lsInput[i].StartsWith(paraStr, StringComparison.OrdinalIgnoreCase))
    //                {
    //                    list.Add(lsInput[i]);
    //                }
    //            }
    //        }
    //        else if (parameterSearch == ParameterSearch.Last)
    //        {
    //            for (Int32 i = 0; i < count; i++)
    //            {
    //                if (lsInput[i].EndsWith(paraStr, StringComparison.OrdinalIgnoreCase))
    //                {
    //                    list.Add(lsInput[i]);
    //                }
    //            }
    //        }
    //        else if (parameterSearch == ParameterSearch.All)
    //        {
    //            for (Int32 i = 0; i < count; i++)
    //            {
    //                if (lsInput[i].IndexOf(paraStr, StringComparison.OrdinalIgnoreCase) >= 0)
    //                {
    //                    list.Add(lsInput[i]);
    //                }
    //            }
    //        }
    //        else if (parameterSearch == ParameterSearch.Same)
    //        {
    //            for (Int32 i = 0; i < count; i++)
    //            {
    //                if (lsInput[i].Equals(paraStr) == true)
    //                {
    //                    list.Add(lsInput[i]);
    //                }
    //            }
    //        }
    //        return list;
    //    }

    //    public ObservableCollection<string> SearchWhenChangePara(string paraStr)
    //    {
    //        if (string.IsNullOrEmpty(paraStr))
    //        {
    //            strOne = string.Empty;
    //            strTwo = string.Empty;
    //            lsOne = null;
    //            lsTwo = null;
    //            return listOriginal;
    //        }

    //        if (paraStr.Length == 1)
    //        {
    //            strTwo = string.Empty;
    //            lsTwo = null;

    //            if (string.Equals(paraStr, strOne))
    //                return lsOne;

    //            strOne = paraStr;
    //            lsOne = SearchPure(listOriginal, strOne);
    //            return lsOne;
    //        }
    //        if (paraStr.Length == 2)
    //        {
    //            if (string.Equals(paraStr, strTwo))
    //                return lsTwo;

    //            strTwo = paraStr;
    //            if (string.IsNullOrEmpty(strOne)
    //                || strOne.ElementAt(0) != strTwo.ElementAt(0))
    //            {
    //                strOne = strTwo.Substring(0, 1);
    //                lsOne = SearchPure(listOriginal, strOne);
    //            }
    //            lsTwo = SearchPure(lsOne, strTwo);
    //            return lsTwo;
    //        }

    //        // paraStr.Length > 2
    //        if (string.IsNullOrEmpty(strOne)
    //                || strOne.ElementAt(0) != paraStr.ElementAt(0))
    //        {
    //            strOne = paraStr.Substring(0, 1);
    //            lsOne = SearchPure(listOriginal, strOne);

    //            strTwo = paraStr.Substring(0, 2);
    //            lsTwo = SearchPure(lsOne, strTwo);
    //        }
    //        else if (strTwo.ElementAt(1) != paraStr.ElementAt(1))
    //        {
    //            strTwo = paraStr.Substring(0, 2);
    //            lsTwo = SearchPure(lsOne, strTwo);
    //        }

    //        return SearchPure(lsTwo, paraStr);
    //    }
    //}
}