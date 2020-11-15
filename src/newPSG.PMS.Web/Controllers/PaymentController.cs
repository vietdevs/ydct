using Abp.Authorization;
using Abp.Runtime.Session;
using newPSG.PMS.KeypayServices;
using newPSG.PMS.Services;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using static newPSG.PMS.CommonENum;

namespace newPSG.PMS.Web.Controllers
{
    [AbpAuthorize]
    public class PaymentController : PMSControllerBase
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

        #endregion Thủ Tục hành Chính

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

        #endregion Thủ Tục hành Chính

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

            #endregion Thủ Tục hành Chính

            _abpSession = abpSession;
        }

        // GET: KeyPay
        [HttpGet]
        public async Task<ActionResult> KeyPayResponse()
        {
            Keypay keypay = new Keypay();
            // lấy tham số để truyền vào hàm
            keypay.Merchant_trans_id = Request.QueryString["merchant_trans_id"];
            keypay.Good_code = Request.QueryString["good_code"];
            keypay.Trans_id = Request.QueryString["trans_id"];
            keypay.Merchant_code = Request.QueryString["merchant_code"];
            //keypay.Merchant_secure_key = string.Empty;
            keypay.Bank_code = Request.QueryString["bank_code"];
            keypay.Command = Request.QueryString["command"];
            keypay.Nest_code = Request.QueryString["net_cost"];
            keypay.Response_code = Request.QueryString["response_code"];
            keypay.Service_code = Request.QueryString["service_code"];
            keypay.Ship_fee = Request.QueryString["ship_fee"];
            keypay.Tax = Request.QueryString["tax"];
            keypay.Current_code = Request.QueryString["currency_code"];
            keypay.Secure_hash = Request.QueryString["secure_hash"];
            if (!string.IsNullOrEmpty(keypay.Good_code) && keypay.Good_code.Contains("."))
            {
                var arr = keypay.Good_code.Split('.');
                var maThuTuc = arr.First();

                //THU_TUC_TT-99
                if (maThuTuc == CommonENum.GetEnumDescription(THU_TUC_ID.THU_TUC_99))
                {
                    await _xuLyHoSoDoanhNghiepAppService.UpdateThanhToanKeyPay(keypay, false);
                    return Redirect("/Application#!/dangkyhoso?kp_statuscode=" + keypay.Response_code);
                }

                #region Thủ Tục hành Chính

                //THU_TUC_TT-01
                else if (maThuTuc == CommonENum.GetEnumDescription(THU_TUC_ID.THU_TUC_01))
                {
                    //await _xuLyHoSoDoanhNghiep01AppService.UpdateThanhToanKeyPay(keypay, false);
                    return Redirect("/Application#!/tt01/dangkyhoso?kp_statuscode=" + keypay.Response_code);
                }
                //THU_TUC_TT-02
                else if (maThuTuc == CommonENum.GetEnumDescription(THU_TUC_ID.THU_TUC_02))
                {
                    //await _xuLyHoSoDoanhNghiep02AppService.UpdateThanhToanKeyPay(keypay, false);
                    return Redirect("/Application#!/tt02/dangkyhoso?kp_statuscode=" + keypay.Response_code);
                }
                //THU_TUC_TT-03
                else if (maThuTuc == CommonENum.GetEnumDescription(THU_TUC_ID.THU_TUC_03))
                {
                    //await _xuLyHoSoDoanhNghiep03AppService.UpdateThanhToanKeyPay(keypay, false);
                    return Redirect("/Application#!/tt03/dangkyhoso?kp_statuscode=" + keypay.Response_code);
                }
                //THU_TUC_TT-04
                else if (maThuTuc == CommonENum.GetEnumDescription(THU_TUC_ID.THU_TUC_04))
                {
                    //await _xuLyHoSoDoanhNghiep04AppService.UpdateThanhToanKeyPay(keypay, false);
                    return Redirect("/Application#!/tt04/dangkyhoso?kp_statuscode=" + keypay.Response_code);
                }
                //THU_TUC_TT-05
                else if (maThuTuc == CommonENum.GetEnumDescription(THU_TUC_ID.THU_TUC_05))
                {
                    //await _xuLyHoSoDoanhNghiep05AppService.UpdateThanhToanKeyPay(keypay, false);
                    return Redirect("/Application#!/tt05/dangkyhoso?kp_statuscode=" + keypay.Response_code);
                }
                //THU_TUC_TT-06
                else if (maThuTuc == CommonENum.GetEnumDescription(THU_TUC_ID.THU_TUC_06))
                {
                    //await _xuLyHoSoDoanhNghiep06AppService.UpdateThanhToanKeyPay(keypay, false);
                    return Redirect("/Application#!/tt06/dangkyhoso?kp_statuscode=" + keypay.Response_code);
                }
                //THU_TUC_TT-07
                else if (maThuTuc == CommonENum.GetEnumDescription(THU_TUC_ID.THU_TUC_07))
                {
                    //await _xuLyHoSoDoanhNghiep07AppService.UpdateThanhToanKeyPay(keypay, false);
                    return Redirect("/Application#!/tt07/dangkyhoso?kp_statuscode=" + keypay.Response_code);
                }
                //THU_TUC_TT-08
                else if (maThuTuc == CommonENum.GetEnumDescription(THU_TUC_ID.THU_TUC_08))
                {
                    //await _xuLyHoSoDoanhNghiep08AppService.UpdateThanhToanKeyPay(keypay, false);
                    return Redirect("/Application#!/tt08/dangkyhoso?kp_statuscode=" + keypay.Response_code);
                }
                //THU_TUC_TT-09
                else if (maThuTuc == CommonENum.GetEnumDescription(THU_TUC_ID.THU_TUC_09))
                {
                    //await _xuLyHoSoDoanhNghiep09AppService.UpdateThanhToanKeyPay(keypay, false);
                    return Redirect("/Application#!/tt09/dangkyhoso?kp_statuscode=" + keypay.Response_code);
                }
                ////THU_TUC_TT-10
                //            else if(maThuTuc == CommonENum.GetEnumDescription(THU_TUC_ID.THU_TUC_10))
                //            {
                //                await _xuLyHoSoDoanhNghiep10AppService.UpdateThanhToanKeyPay(keypay, false);
                //                return Redirect("/Application#!/tt10/dangkyhoso?kp_statuscode=" + keypay.Response_code);
                //            }

                #endregion Thủ Tục hành Chính
            }
            return Redirect("/Application#!/tenant/dashboard");
        }
    }
}