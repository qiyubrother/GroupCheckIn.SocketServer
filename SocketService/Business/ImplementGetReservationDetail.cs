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
    /// 取得预定房间明细
    /// </summary>
    public class ImplementGetReservationDetail: ImplementTransaction
    {
        public ImplementGetReservationDetail(ITransaction data)
        {
            Tx = data as ITransactionTx;
        }

        public override ITransaction Action()
        {
            var tx = Tx as GetReservationDetailTx;
            var rx = new GetReservationDetailRx();
            #region Check
            if (tx.ReservationNo == string.Empty)
            {
                rx.ErrorCode = "-4";
                rx.ErrorMessage = "ReservationNo is empty.";
                return Rx = rx;
            }
            if (tx.HotelId == string.Empty)
            {
                rx.ErrorCode = "-5";
                rx.ErrorMessage = "Invalid HotelId.";
                return Rx = rx;
            }
            #endregion

            rx.ReservationRooms = GetRoomReservationInfo(tx.HotelId, tx.ReservationNo);
            rx.ErrorCode = "0";
            rx.ErrorMessage = "Successful";
            return Rx = rx;
        }
        /// <summary>
        /// 取得预定详细信息
        /// </summary>
        /// <returns></returns>
        public IEnumerable<RoomReservationInfo> GetRoomReservationInfo(string hotelId, string reservationNo)
        {
            return new List<RoomReservationInfo>()
            {
                new RoomReservationInfo
                {
                     RoomNo="401",
                     RoomType= RoomType.Single,
                     SubReservationNo = "S12345600001"
                },
                new RoomReservationInfo
                {
                     RoomNo="502",
                     RoomType= RoomType.Standard,
                     SubReservationNo = "S12345600002"
                },
                new RoomReservationInfo
                {
                     RoomNo="303",
                     RoomType= RoomType.Single,
                     SubReservationNo = "S12345600003"
                },
                new RoomReservationInfo
                {
                     RoomNo="302",
                     RoomType= RoomType.Standard,
                     SubReservationNo = "S12345600004"
                },
            };
        }
    }
}
