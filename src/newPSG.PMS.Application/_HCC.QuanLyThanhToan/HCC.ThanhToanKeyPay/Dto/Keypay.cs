using newPSG.PMS.Configuration;
using newPSG.PMS.vn.keypay.webservices;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace newPSG.PMS.KeypayServices
{
    public class Keypay
    {
        public Keypay()
        {

        }
        public string Merchant_secure_key { get; set; }
        public string Bank_code { get; set; }
        public string Nest_code { get; set; }
        public string Version { get; set; }
        public string Command { get; set; }
        public string Current_local { get; set; }
        public string Merchant_code { get; set; }
        public string Current_code { get; set; }
        public string Desc1 { get; set; }
        public string Desc2 { get; set; }
        public string Desc3 { get; set; }
        public string Desc4 { get; set; }
        public string Desc5 { get; set; }
        public string Xdescription { get; set; }
        public string Service_code { get; set; }
        public string Keypayurl { get; set; }
        public string Merchant_name { get; set; }
        public string Secure_hash { get; set; }
        public string Internal_bank { get; set; }
        public string Response_code { get; set; }
        public string Merchant_trans_id { get; set; }
        public string Good_code { get; set; }
        public string Trans_id { get; set; }
        public string Ship_fee { get; set; }
        public string Tax { get; set; }
        public string Return_url { get; set; }
        public string Country_Code { get; set; }
        public string RequestJson { get; set; }
        public string Trans_time { get; set; } //Định dạng "YYYYMMDD"

        #region Build Url Call Keypay
        //tạo url thanh toán
        public string buildPayUrl()
        {
            // encoding
            Encoding iso = Encoding.GetEncoding("iso8859-1");
            Encoding utf8 = Encoding.UTF8;

            byte[] description_byte = utf8.GetBytes(this.Xdescription);
            byte[] country_Code_byte = utf8.GetBytes(this.Country_Code);
            byte[] desc1_byte = utf8.GetBytes(this.Desc1);
            byte[] desc2_byte = utf8.GetBytes(this.Desc2);
            byte[] desc3_byte = utf8.GetBytes(this.Desc3);
            byte[] desc4_byte = utf8.GetBytes(this.Desc4);

            string sxml_des = iso.GetString(description_byte);
            string scountrycode = iso.GetString(country_Code_byte);
            string sdesc1 = iso.GetString(desc1_byte);
            string sdesc2 = iso.GetString(desc2_byte);
            string sdesc3 = iso.GetString(desc3_byte);
            string sdesc4 = iso.GetString(desc4_byte);

            string description = HttpUtility.UrlEncode(sxml_des, utf8);
            string country_Code = HttpUtility.UrlEncode(scountrycode, utf8);
            string desc1 = HttpUtility.UrlEncode(sdesc1, utf8);
            string desc2 = HttpUtility.UrlEncode(sdesc1, utf8);
            string desc3 = HttpUtility.UrlEncode(sdesc1, utf8);
            string desc4 = HttpUtility.UrlEncode(sdesc1, utf8);

            var _request = new {
                this.Bank_code,
                this.Command,
                this.Country_Code,
                this.Current_code,
                this.Current_local,
                this.Desc1,
                this.Desc2,
                this.Desc3,
                this.Desc4,
                this.Desc5,
                this.Good_code,
                this.Internal_bank,
                this.Merchant_trans_id,
                this.Merchant_code,
                this.Nest_code,
                this.Return_url,
                this.Service_code,
                this.Ship_fee,
                this.Secure_hash,
                this.Tax,
                this.Xdescription,
                this.Version
            };
            this.RequestJson = JsonConvert.SerializeObject(_request);
            // url to pay
            return this.Keypayurl + "?bank_code=" + this.Bank_code + "&command=" + this.Command + "&country_code="
               + country_Code + "&currency_code=" + this.Current_code + "&current_locale=" + this.Current_local + "&desc_1="
               + desc1 + "&desc_2=" + desc2 + "&desc_3=" + desc3 + "&desc_4=" + desc4 + "&desc_5=" + this.Desc5 + "&good_code="
               + this.Good_code + "&internal_bank=" + this.Internal_bank + "&merchant_trans_id=" + this.Merchant_trans_id + "&merchant_code="
               + this.Merchant_code + "&net_cost=" + this.Nest_code + "&return_url=" + this.Return_url + "&service_code=" + this.Service_code + "&ship_fee="
               + this.Ship_fee + "&secure_hash=" + this.Secure_hash + "&tax=" + this.Tax + "&xml_description=" + description + "&version=" + this.Version;
        }

        //get message from reponse code KP
        public string genMsgReturn(string response_code)
        {
            string msg;
            switch (Int32.Parse(response_code))
            {
                case 0:
                    //success
                    msg = "Thanh toán thành công";
                    break;
                case 1:
                    //merchant code sai
                    msg = "Đại lý không tồn tại trong hệ thống";
                    break;
                case 2:
                    //chuoi ma hoa ko hop le
                    msg = "Chuỗi mã hóa không hợp lệ";
                    break;
                case 3:
                    //merchant trans id sai
                    msg = "Mã giao dịch không hợp lệ";
                    break;
                case 4:
                    //trans id khong ton tai
                    msg = "Đã có lỗi xảy ra khi thực hiện thanh toán";
                    break;
                case 5:
                    //so tien chu the ko du thuc hien giao dich
                    msg = "Mã dịch vụ không hợp lệ";
                    break;
                case 6:
                    //giao dich da gui confirm
                    msg = "Giao dịch đã tồn tại trong hệ thống";
                    break;
                case 7:
                    msg = "Mã quốc gia không hợp lệ";
                    break;
                case 8:
                    //timeout
                    msg = "Không nhận được kết quả trả về từ Ngân hàng do quá thời gian thực hiện giao dịch";
                    break;
                case 9:
                    msg = "Mô tả đơn hàng không hợp lệ";
                    break;
                case 10:
                    msg = "Mã đơn hàng không hợp lệ";
                    break;
                case 11:
                    //net cost fail
                    msg = "Số tiền thanh toán không hợp lệ";
                    break;
                case 12:
                    //ship fee fail
                    msg = "Phí vận chuyển không hợp lệ";
                    break;
                case 13:
                    //tax fail
                    msg = "Thuế không hợp lệ";
                    break;
                case 14:
                    //merchant code chua duoc cau hinh de thanh toan
                    msg = "Đã có lỗi xảy ra khi thực hiện thanh toán";
                    break;
                case 15:
                    //sai ma ngan hang
                    msg = "Mã ngân hàng không nằm trong hệ thống chấp nhận thanh toán";
                    break;
                case 16:
                    //so tien dai ly ko nam trong khoang cho phep (dai ly o day la keypay so voi BN)
                    msg = "Số tiền thanh toán không nằm trong khoảng cho phép";
                    break;
                case 17:
                    //tai khoan ko du tien
                    msg = "Tài khoản không đủ tiền thực hiện giao dịch";
                    break;
                case 18:
                    //huy bo giao dich
                    msg = "Bạn đã hủy bỏ giao dịch";
                    break;
                case 19:
                    //trans date time ko hop le
                    msg = "Thời gian thực hiện thanh toán không hợp lệ";
                    break;
                case 20:
                    //otp type ko hop le
                    msg = "Kiểu nhận mã OTP không đúng";
                    break;
                case 21:
                    //otp nhap ko dung
                    msg = "Xác thực OTP không thành công";
                    break;
                case 24:
                    //ko tim thay giao dich trong BN
                    msg = "Không tồn tại giao dịch trong hệ thống";
                    break;
                case 25:
                    //sai thong tin chu the lan 1
                    msg = "Nhập sai thông tin chủ thẻ lần 1";
                    break;
                case 26:
                    //sai thong tin chu the lan 2
                    msg = "Nhập sai thông tin chủ thẻ lần 2";
                    break;
                case 27:
                    //sai thong tin chu the lan 3 - redirect
                    msg = "Nhập sai thông tin chủ thẻ lần 3";
                    break;
                case 28:
                    // sai so the
                    msg = "Invalid Card number";
                    break;
                case 29:
                    // sai ngay hieu luc the
                    msg = "Invalid Card date";
                    break;
                case 30:
                    msg = "Phiên bản thanh toán không hợp lệ";
                    break;
                case 31:
                    msg = "Yêu cầu không hợp lệ";
                    break;
                case 32:
                    msg = "Loại tiền tệ không hợp lệ";
                    break;
                case 33:
                    msg = "Locale không hợp lệ";
                    break;
                case 34:
                    msg = "Desc_1 không hợp lệ";
                    break;
                case 35:
                    msg = "Desc_2 không hợp lệ";
                    break;
                case 36:
                    msg = "Desc_3 không hợp lệ";
                    break;
                case 37:
                    msg = "Desc_4 không hợp lệ";
                    break;
                case 38:
                    msg = "Desc_5 không hợp lệ";
                    break;
                case 39:
                    msg = "Return_url không hợp lệ";
                    break;
                case 41:
                    //the nghi van
                    msg = "Phát hiện thẻ nghi vấn";
                    break;
                case 54:
                    //the het han
                    msg = "Thẻ ngân hàng hết hạn";
                    break;
                case 57:
                    //chua dang ky ibanking
                    msg = "Thẻ ngân hàng chưa đăng ký dịch vụ thanh toán trực tuyến";
                    break;
                case 61:
                    //qua han muc giao dich trong ngay
                    msg = "Thẻ sử dụng đã quá hạn mức giao dịch trong ngày";
                    break;
                case 62:
                    //the bi khoa
                    msg = "Thẻ bị khóa";
                    break;
                case 65:
                    //qua han muc giao dich trong 1 lan gdich
                    msg = "Thẻ quá hạn mức trong một lần giao dịch";
                    break;
                case 66:
                    //qua han muc giao dich 
                    msg = "Too limits the transaction";
                    break;
                case 97:
                    //bank chua san sang
                    msg = "Kết nối tới Ngân Hàng không thành công";
                    break;
                case 98:
                    //giao dich ko hop le
                    msg = "Giao dịch không được phép";
                    break;
                case 99:
                    //loi ko xac dinh
                    msg = "Đã có lỗi xảy ra khi thực hiện thanh toán";
                    break;
                default:
                    //loi ko xac dinh
                    msg = "Đã có lỗi xảy ra khi thực hiện thanh toán";
                    break;
            }
            return msg;
        }
        #endregion

        #region Các Hàm mã hóa Key
        // tính securehash keypay
        public string get_Secure_Hash_SHA()
        {
            Secure_Hash_SHA sha = new Secure_Hash_SHA();
            return sha.GetSHAHash(this.Merchant_secure_key + this.Command + this.Country_Code
                    + this.Current_code + this.Current_local + this.Good_code + this.Merchant_code + this.Merchant_trans_id
                    + this.Nest_code + this.Return_url + this.Service_code + this.Ship_fee + this.Tax + this.Version).ToUpper();
        }
        // tính securehash merchant
        public string get_MerchantSecure_Hash_SHA()
        {
            Secure_Hash_SHA sha = new Secure_Hash_SHA();
            return sha.GetSHAHash(this.Merchant_secure_key + this.Bank_code + this.Command
                    + this.Current_code + this.Good_code + this.Merchant_code + this.Merchant_trans_id
                    + this.Nest_code + this.Response_code + this.Service_code + this.Ship_fee + this.Tax + this.Trans_id).ToUpper();
        }
        
        // tính securehash keypay
        public string get_Secure_Hash_MD5()
        {
            Secure_Hash_MD5 md5 = new Secure_Hash_MD5();
            return md5.GetMD5Hash(this.Merchant_secure_key + this.Command + this.Country_Code
                    + this.Current_code + this.Current_local + this.Good_code + this.Merchant_code + this.Merchant_trans_id
                    + this.Nest_code + this.Return_url + this.Service_code + this.Ship_fee + this.Tax + this.Version).ToUpper();
        }
        // tính securehash merchant
        public string get_MerchantSecure_Hash_MD5()
        {
            Secure_Hash_MD5 md5 = new Secure_Hash_MD5();
            return md5.GetMD5Hash(this.Merchant_secure_key + this.Bank_code + this.Command
                    + this.Current_code + this.Good_code + this.Merchant_code + this.Merchant_trans_id
                    + this.Nest_code + this.Response_code + this.Service_code + this.Ship_fee + this.Tax + this.Trans_id).ToUpper();
        }
        #endregion
    }
    public class Secure_Hash_SHA
    {
        public Secure_Hash_SHA()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        // Mothod hash SHA
        public string GetSHAHash(string input)
        {
            //System.Security.Cryptography.SHA256 sha256 = new System.Security.Cryptography.SHA256Managed();
            //byte[] sha256Bytes = System.Text.Encoding.Default.GetBytes(input);
            //byte[] cryString = sha256.ComputeHash(sha256Bytes);
            //string sha256Str = string.Empty;
            //for (int i = 0; i < cryString.Length; i++)
            //{
            //    sha256Str += cryString[i].ToString("X");
            //}
            //return sha256Str;

            System.Security.Cryptography.SHA256 x = new System.Security.Cryptography.SHA256Managed();
            byte[] bs = System.Text.Encoding.UTF8.GetBytes(input);
            bs = x.ComputeHash(bs);
            System.Text.StringBuilder s = new System.Text.StringBuilder();
            foreach (byte b in bs)
            {
                s.Append(b.ToString("x2").ToLower());
            }
            string password = s.ToString();
            return password;
        }
    }
    public class Secure_Hash_MD5
    {
        public string GetMD5Hash(string input)
        {
            System.Security.Cryptography.MD5CryptoServiceProvider x = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] bs = System.Text.Encoding.UTF8.GetBytes(input);
            bs = x.ComputeHash(bs);
            System.Text.StringBuilder s = new System.Text.StringBuilder();
            foreach (byte b in bs)
            {
                s.Append(b.ToString("x2").ToLower());
            }
            string password = s.ToString();
            return password;
        }
    }
}
