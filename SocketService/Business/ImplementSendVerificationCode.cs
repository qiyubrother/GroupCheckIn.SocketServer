using System;
using System.Collections.Generic;
using System.Text;
using Transaction.Definition;
using Transaction;
using DataModule.Model;
using System.Linq;
namespace SocketService.Business
{
    /// <summary>
    /// 发送验证码
    /// </summary>
    public class ImplementSendVerificationCode : ImplementTransaction
    {
        public ImplementSendVerificationCode(ITransaction data)
        {
            Tx = data as ITransactionTx;
        }
        public override ITransaction Action()
        {
            var tx = Tx as SendVerificationCodeTx;
            var rx = new SendVerificationCodeRx();
            #region Check
            if (string.IsNullOrEmpty(tx.Mobile))
            {
                rx.ErrorCode = "-1";
                rx.ErrorMessage = "Mobile is empty.";
                return Rx = rx;
            }
            if (string.IsNullOrEmpty(tx.ReservationNo))
            {
                rx.ErrorCode = "-4";
                rx.ErrorMessage = "ReservationNo is empty.";
                return Rx = rx;
            }
            if (string.IsNullOrEmpty(tx.HotelId))
            {
                rx.ErrorCode = "-5";
                rx.ErrorMessage = "Invalid HotelId.";
                return Rx = rx;
            }
            if (tx.Mobile.Length != 11)
            {
                rx.ErrorCode = "-6";
                rx.ErrorMessage = "Invalid mobile.";
                return Rx = rx;
            }
            #endregion
            var rst = SendVerificationCode(tx.Mobile);
            if (rst.IsOK)
            {
                using (var dc = new DataContext())
                {
                    var au = new Authentication { HotelId = tx.HotelId, Mobile = tx.Mobile, ReservationNo = tx.ReservationNo, VerificationCode = rst.VerificationCode, Expiration = DateTime.Now.AddMinutes(3) };
                    var item = dc.Authentications.FirstOrDefault(x => x.HotelId == tx.HotelId && x.Mobile == tx.Mobile && x.ReservationNo == tx.ReservationNo);
                    if (item != null)
                    {
                        dc.Authentications.Remove(item);
                    }
                    dc.Authentications.Add(au);
                    try
                    {
                        dc.SaveChanges();
                        rx.ErrorCode = "0";
                        rx.ErrorMessage = "Successful";
                    }
                    catch(Exception ex)
                    {
                        rx.ErrorCode = "-11";
                        rx.ErrorMessage = ex.Message.Replace("\"", string.Empty);
                    }
                }
            }
            else
            {
                rx.ErrorCode = "-10";
                rx.ErrorMessage = "Send verification code failed.";
            }

            return Rx = rx;
        }

        /// <summary>
        /// 向指定手机发送验证码
        /// </summary>
        /// <param name="mobile"></param>
        /// <returns>返回发送的验证码</returns>
        private SendVerificationCodeResult SendVerificationCode(string mobile)
        {
            var rst = new SendVerificationCodeResult { IsOK = true };
            rst.VerificationCode = Guid.NewGuid().ToString().Substring(0, 6);

            return rst;
        }

        class SendVerificationCodeResult
        {
            public bool IsOK { get; set; }
            public string VerificationCode { get; set; }
        }
    }
}
