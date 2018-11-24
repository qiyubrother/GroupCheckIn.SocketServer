using System;
using System.Collections.Generic;
using System.Text;
using SocketService.Business;
using Transaction.Definition;
using Transaction;
using DataModule.Model;
using System.Linq;

namespace SocketService.Business
{
    /// <summary>
    /// 验证发送的验证码是否正确
    /// </summary>
    public class ImplementAuthentications : ImplementTransaction
    {
        public ImplementAuthentications(ITransaction data)
        {
            Tx = data as ITransactionTx;
        }
        public override ITransaction Action()
        {
            var tx = Tx as AuthenticationsTx;
            var rx = new AuthenticationsRx();

            using (var dc = new DataContext())
            {
                if (dc.Authentications.Any(x=>x.HotelId == tx.HotelId 
                    && x.Mobile == tx.Mobile 
                    && x.VerificationCode == tx.VerificationCode
                    && x.Expiration > DateTime.Now))
                {
                    rx.ErrorCode = "0";
                    rx.ErrorMessage = "Successful";
                }
                else
                {
                    rx.ErrorCode = "-1";
                    rx.ErrorMessage = "Invalid verification code.";
                }
            }
            return Rx = rx;
        }
    }
}
