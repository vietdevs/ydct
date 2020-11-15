using Abp.Runtime.Session;
using newPSG.PMS.Common;
using newPSG.PMS.KeypayServices;
using newPSG.PMS.Services;
using newPSG.PMS.WebApi.Models;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Script.Serialization;
using static newPSG.PMS.CommonENum;

namespace newPSG.PMS.WebApi.Controllers
{
    public class PaymentController : PMSApiControllerBase
    {
        private readonly IXuLyHoSoDoanhNghiep99AppService _xuLyHoSoDoanhNghiepAppService;
        #region Thủ Tục hành Chính
        //private readonly IXuLyHoSoDoanhNghiep01AppService _xuLyHoSoDoanhNghiep01AppService;
        //private readonly IXuLyHoSoDoanhNghiep02AppService _xuLyHoSoDoanhNghiep02AppService;
        //private readonly IXuLyHoSoDoanhNghiep03AppService _xuLyHoSoDoanhNghiep03AppService;
        //private readonly IXuLyHoSoDoanhNghiep04AppService _xuLyHoSoDoanhNghiep04AppService;
        //private readonly IXuLyHoSoDoanhNghiep05AppService _xuLyHoSoDoanhNghiep05AppService;
        //private readonly IXuLyHoSoDoanhNghiep06AppService _xuLyHoSoDoanhNghiep06AppService;
        //private readonly IXuLyHoSoDoanhNghiep07AppService _xuLyHoSoDoanhNghiep07AppService;
        //private readonly IXuLyHoSoDoanhNghiep08AppService _xuLyHoSoDoanhNghiep08AppService;
        //private readonly IXuLyHoSoDoanhNghiep09AppService _xuLyHoSoDoanhNghiep09AppService;
        //private readonly IXuLyHoSoDoanhNghiep10AppService _xuLyHoSoDoanhNghiep10AppService;
        #endregion
        private readonly IAbpSession _abpSession;
        public PaymentController(
            IXuLyHoSoDoanhNghiep99AppService xuLyHoSoDoanhNghiepAppService,
        #region Thủ Tục hành Chính
            //IXuLyHoSoDoanhNghiep01AppService xuLyHoSoDoanhNghiep01AppService,
            //IXuLyHoSoDoanhNghiep02AppService xuLyHoSoDoanhNghiep02AppService,
            //IXuLyHoSoDoanhNghiep03AppService xuLyHoSoDoanhNghiep03AppService,
            //IXuLyHoSoDoanhNghiep04AppService xuLyHoSoDoanhNghiep04AppService,
            //IXuLyHoSoDoanhNghiep05AppService xuLyHoSoDoanhNghiep05AppService,
            //IXuLyHoSoDoanhNghiep06AppService xuLyHoSoDoanhNghiep06AppService,
            //IXuLyHoSoDoanhNghiep07AppService xuLyHoSoDoanhNghiep07AppService,
            //IXuLyHoSoDoanhNghiep08AppService xuLyHoSoDoanhNghiep08AppService,
            //IXuLyHoSoDoanhNghiep09AppService xuLyHoSoDoanhNghiep09AppService,
            //IXuLyHoSoDoanhNghiep10AppService xuLyHoSoDoanhNghiep10AppService,
        #endregion
            IAbpSession abpSession
            )
        {
            _xuLyHoSoDoanhNghiepAppService = xuLyHoSoDoanhNghiepAppService;
            #region Thủ Tục hành Chính
            //_xuLyHoSoDoanhNghiep01AppService = xuLyHoSoDoanhNghiep01AppService;
            //_xuLyHoSoDoanhNghiep02AppService = xuLyHoSoDoanhNghiep02AppService;
            //_xuLyHoSoDoanhNghiep03AppService = xuLyHoSoDoanhNghiep03AppService;
            //_xuLyHoSoDoanhNghiep04AppService = xuLyHoSoDoanhNghiep04AppService;
            //_xuLyHoSoDoanhNghiep05AppService = xuLyHoSoDoanhNghiep05AppService;
            //_xuLyHoSoDoanhNghiep06AppService = xuLyHoSoDoanhNghiep06AppService;
            //_xuLyHoSoDoanhNghiep07AppService = xuLyHoSoDoanhNghiep07AppService;
            //_xuLyHoSoDoanhNghiep08AppService = xuLyHoSoDoanhNghiep08AppService;
            //_xuLyHoSoDoanhNghiep09AppService = xuLyHoSoDoanhNghiep09AppService;
            //_xuLyHoSoDoanhNghiep10AppService = xuLyHoSoDoanhNghiep10AppService;
            #endregion
            _abpSession = abpSession;
        }

        #region API KeyPay
        [HttpGet]
        [Route("api/payment/keypay_result_ipn")]
        public void KeyPayResultIPN()
        {
            var data = Request.RequestUri.ParseQueryString();
            Keypay keypay = new Keypay();
            // lấy tham số để truyền vào hàm
            keypay.Merchant_trans_id = data["merchant_trans_id"];
            keypay.Good_code = data["good_code"];
            keypay.Trans_id = data["trans_id"];
            keypay.Merchant_code = data["merchant_code"];
            keypay.Merchant_secure_key = string.Empty;
            keypay.Bank_code = data["bank_code"];
            keypay.Command = data["command"];
            keypay.Nest_code = data["net_cost"];
            keypay.Response_code = data["response_code"];
            keypay.Service_code = data["service_code"];
            keypay.Ship_fee = data["ship_fee"];
            keypay.Tax = data["tax"];
            keypay.Current_code = data["currency_code"];
            keypay.Secure_hash = data["secure_hash"];

            if (!string.IsNullOrEmpty(keypay.Good_code) && keypay.Good_code.Contains("."))
            {
                var arr = keypay.Good_code.Split('.');
                var maThuTuc = arr.First();

                //THU_TUC_TT-99
                if (maThuTuc == CommonENum.GetEnumDescription(THU_TUC_ID.THU_TUC_99))
                {
                    _xuLyHoSoDoanhNghiepAppService.UpdateThanhToanKeyPay(keypay, true);
                }
                #region Thủ Tục hành Chính
                //THU_TUC_TT-01
                else if (maThuTuc == CommonENum.GetEnumDescription(THU_TUC_ID.THU_TUC_01))
                {
                    //_xuLyHoSoDoanhNghiep01AppService.UpdateThanhToanKeyPay(keypay, true);
                }
                //THU_TUC_TT-02
                else if (maThuTuc == CommonENum.GetEnumDescription(THU_TUC_ID.THU_TUC_02))
                {
                    //_xuLyHoSoDoanhNghiep02AppService.UpdateThanhToanKeyPay(keypay, true);
                }
                //THU_TUC_TT-03
                else if (maThuTuc == CommonENum.GetEnumDescription(THU_TUC_ID.THU_TUC_03))
                {
                    //_xuLyHoSoDoanhNghiep03AppService.UpdateThanhToanKeyPay(keypay, true);
                }
                //THU_TUC_TT-04
                else if (maThuTuc == CommonENum.GetEnumDescription(THU_TUC_ID.THU_TUC_04))
                {
                    //_xuLyHoSoDoanhNghiep04AppService.UpdateThanhToanKeyPay(keypay, true);
                }
                //THU_TUC_TT-05
                else if (maThuTuc == CommonENum.GetEnumDescription(THU_TUC_ID.THU_TUC_05))
                {
                    //_xuLyHoSoDoanhNghiep05AppService.UpdateThanhToanKeyPay(keypay, true);
                }
                //THU_TUC_TT-06
                else if (maThuTuc == CommonENum.GetEnumDescription(THU_TUC_ID.THU_TUC_06))
                {
                    //_xuLyHoSoDoanhNghiep06AppService.UpdateThanhToanKeyPay(keypay, true);
                }
                //THU_TUC_TT-07
                else if (maThuTuc == CommonENum.GetEnumDescription(THU_TUC_ID.THU_TUC_07))
                {
                    //_xuLyHoSoDoanhNghiep07AppService.UpdateThanhToanKeyPay(keypay, true);
                }
                //THU_TUC_TT-08
                else if (maThuTuc == CommonENum.GetEnumDescription(THU_TUC_ID.THU_TUC_08))
                {
                    //_xuLyHoSoDoanhNghiep08AppService.UpdateThanhToanKeyPay(keypay, true);
                }
                //THU_TUC_TT-09
                else if (maThuTuc == CommonENum.GetEnumDescription(THU_TUC_ID.THU_TUC_09))
                {
                    //_xuLyHoSoDoanhNghiep09AppService.UpdateThanhToanKeyPay(keypay, true);
                }
                ////THU_TUC_TT-10
                //else if (maThuTuc == CommonENum.GetEnumDescription(THU_TUC_ID.THU_TUC_10))
                //{
                //    _xuLyHoSoDoanhNghiep09AppService.UpdateThanhToanKeyPay(keypay, true);
                //}
                #endregion
            }
        }
        #endregion
    }
}