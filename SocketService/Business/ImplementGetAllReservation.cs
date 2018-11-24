using System;
using System.Collections.Generic;
using System.Text;
using Transaction.Definition;
using Transaction;
using DataModule.Model;
using System.Linq;
using System.Globalization;

namespace SocketService.Business
{
    /// <summary>
    /// 取得所有预定
    /// </summary>
    public class ImplementGetAllReservation : ImplementTransaction
    {
        public ImplementGetAllReservation(ITransaction data)
        {
            Tx = data as ITransactionTx;
        }

        public override ITransaction Action()
        {
            var tx = Tx as GetAllReservationTx;
            var rx = new GetAllReservationRx();
            #region Check
            if (string.IsNullOrEmpty(tx.HotelId))
            {
                rx.ErrorCode = "-5";
                rx.ErrorMessage = "Invalid HotelId.";
                return Rx = rx;
            }
            #endregion
            using (var dc = new DataContext())
            {
                var lst = new List<ReservationInfo>();

                if (tx.Status == "Valid")
                {
                    var q = dc.Reservations.Where(x => DateTime.ParseExact(x.CheckInDate, "yyyy/MM/dd", null) >= DateTime.Now && x.HotelId == tx.HotelId).ToList();
                    foreach (var x in q)
                    {
                        lst.Add(new ReservationInfo
                        {
                            HotelId = x.HotelId,
                            Mobile = x.Mobile,
                            ReservationNo = x.ReservationNo,
                            ReservationBy = x.ReservationBy,
                            CheckInDate = x.CheckInDate,
                            CreateDate = x.CreateDate
                        });
                    }
                }
                else
                {
                    var q = dc.Reservations.Where(x => DateTime.ParseExact(x.CheckInDate, "yyyy/MM/dd", null) < DateTime.Now && x.HotelId == tx.HotelId);
                    foreach (var x in q)
                    {
                        lst.Add(new ReservationInfo
                        {
                            HotelId = x.HotelId,
                            Mobile = x.Mobile,
                            ReservationNo = x.ReservationNo,
                            ReservationBy = x.ReservationBy,
                            CheckInDate = x.CheckInDate,
                            CreateDate = x.CreateDate
                        });
                    }
                }

                rx.Reservations = lst;
                rx.ErrorCode = "0";
                rx.ErrorMessage = "Successful";
            }

            return Rx = rx;
        }
    }
}
