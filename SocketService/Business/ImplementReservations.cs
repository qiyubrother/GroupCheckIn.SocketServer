using System;
using System.Collections.Generic;
using System.Text;
using SocketService.Business;
using Transaction.Definition;
using Transaction;
using DataModule.Model;
using Newtonsoft.Json;
using System.Globalization;

namespace SocketService.Business
{
    /// <summary>
    /// 添加预定
    /// </summary>
    public class ImplementReservations : ImplementTransaction
    {
        public ImplementReservations(ITransaction data)
        {
            Tx = data as ITransactionTx;
        }
        /// <summary>
        /// 实现业务逻辑::添加预定
        /// </summary>
        /// <returns></returns>
        public override ITransaction Action()
        {
            var tx = Tx as ReservationsTx;
            var rx = new ReservationsRx { HotelId = tx.HotelId};
            #region Check
            if (string.IsNullOrEmpty(tx.Mobile))
            {
                rx.ErrorCode = "-1";
                rx.ErrorMessage = "Mobile is empty.";
                return Rx = rx;
            }
            if (string.IsNullOrEmpty(tx.ReservationBy))
            {
                rx.ErrorCode = "-2";
                rx.ErrorMessage = "ReservationBy is empty.";
                return Rx = rx;
            }
            if (string.IsNullOrEmpty(tx.CheckInDate))
            {
                rx.ErrorCode = "-3";
                rx.ErrorMessage = "Check in date is empty.";
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
            try
            {
                DateTime dt = DateTime.ParseExact(tx.CheckInDate, "yyyy/MM/dd", null);
            }
            catch
            {
                rx.ErrorCode = "-7";
                rx.ErrorMessage = "Invalid check in Date.";
                return Rx = rx;
            }
            #endregion
            LogHelper.WriteLogAsync($"[{DateTime.Now}][SocketServer][数据验证通过]{JsonConvert.SerializeObject(tx)}", LogType.All);
            Console.WriteLine(JsonConvert.SerializeObject(tx));

            using (var dc = new DataContext())
            {
                var res = new Reservation
                {
                    HotelId = tx.HotelId,
                    CreateDate = DateTime.Now.ToString(),
                    Mobile = tx.Mobile,
                    CheckInDate = tx.CheckInDate,
                    ReservationBy = tx.ReservationBy,
                    ReservationNo = tx.ReservationNo
                };

                dc.Reservations.Add(res);
                try
                {
                    dc.SaveChanges();
                    LogHelper.WriteLogAsync($"[{DateTime.Now}][SocketServer][数据保存成功]", LogType.All);
                    rx.ErrorCode = "0";
                    rx.ErrorMessage = "Successful";
                }
                catch(Exception ex)
                {
                    LogHelper.WriteLogAsync($"[{DateTime.Now}][SocketServer][保存数据异常]{ex.Message}", LogType.All);
                    rx.ErrorCode = "-11";
                    rx.ErrorMessage = ex.Message.Replace("\"", string.Empty);
                }
            }
            Rx = rx;
            LogHelper.WriteLogAsync($"[{DateTime.Now}][SocketServer][返回处理结果Rx]{JsonConvert.SerializeObject(Rx)}", LogType.All);

            return Rx;
        }
    }
}
