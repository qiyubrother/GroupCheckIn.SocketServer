using System;
using SocketCommunicationFoundation;
using System.Text;
using System.Net;
using Transaction;
using Newtonsoft.Json;
using SocketService.Business;
using Transaction.Definition;

namespace SocketService
{
    public class CoreService
    {
        public void Start()
        {
            LogHelper.LogFilePath = @"\svrLog";
            Func<string, object> ImplementAction = (s) =>
            {
                var code = (JsonConvert.DeserializeObject(s) as Newtonsoft.Json.Linq.JObject).GetValue("TransCode");
                switch (code.ToString())
                {
                    #region 添加预定
                    case "100001":
                        {
                            var a = new ImplementReservations(JsonConvert.DeserializeObject<ReservationsTx>(s));
                            a.Action();
                            return a.Rx;
                        }
                    #endregion
                    #region 验证验证码上送报文定义
                    case "100002":
                        {
                            var a = new ImplementAuthentications(JsonConvert.DeserializeObject<AuthenticationsTx>(s));
                            a.Action();
                            return a.Rx;
                        }
                    #endregion
                    #region 发送验证码上送报文定义
                    case "100003":
                        {
                            var a = new ImplementSendVerificationCode(JsonConvert.DeserializeObject<SendVerificationCodeTx>(s));
                            a.Action();
                            return a.Rx;
                        }
                    #endregion
                    #region 取得预定房间明细上送报文
                    case "100004":
                        {
                            var a = new ImplementGetReservationDetail(JsonConvert.DeserializeObject<GetReservationDetailTx>(s));
                            a.Action();
                            return a.Rx;
                        }
                    #endregion
                    #region 取得所有预定上送报文定义
                    case "100005":
                        {
                            var a = new ImplementGetAllReservation(JsonConvert.DeserializeObject<GetAllReservationTx>(s));
                            a.Action();
                            return a.Rx;
                        }
                    #endregion
                    default:
                        return null;
                }
            };

            Server.Startup(
                new IPEndPoint(new IPAddress(new byte[] { 192, 168, 0, 6 }), 5060),
                (o)=>
                {
                    #region 植入命令行插件
                    var param = o as SocketCallbackEventArgs;
                    CommandLineProcess.Do(o, "#", Encoding.Default, (obj, cmdList, lastCommand) =>
                    {
                        LogHelper.WriteLogAsync($"[{DateTime.Now}][SocketServer]收到SocketClient发来的数据。准备处理...", LogType.All);
                        var txData = string.Empty;
                        foreach (var cmd in cmdList)
                        {
                            try
                            {
                                #region Base64解密
                                txData = qiyubrother.extend.Base64.DecodeString(cmd);
                                #endregion
                                #region AES解密数据（Hold）

                                #endregion
                                #region 处理上送报文并取得返回结果
                                var sData = JsonConvert.SerializeObject(ImplementAction(txData));
                                #endregion
                                #region AES加密数据（Hold）
                                LogHelper.WriteLogAsync($"[{DateTime.Now}][SocketServer]处理成功。{sData}", LogType.All);
                                #endregion
                                #region Base64加密
                                var rxData = qiyubrother.extend.Base64.EncodeString(sData, Encoding.UTF8);
                                #endregion
                                #region 返回处理结果
                                param.Socket.Send(Encoding.UTF8.GetBytes(rxData));
                                LogHelper.WriteLogAsync($"[{DateTime.Now}]已经将结果返回。{rxData}", LogType.All);
                                #endregion
                            }
                            catch (Exception ex)
                            {
                                // 出错后，继续解析下一条命令
                                LogHelper.WriteLogAsync($"[{DateTime.Now}][-99]{ex.Message}", LogType.All);
                            }
                        }
                    });
                    #endregion
                });

            Console.WriteLine("Socket server is running. IP:192.168.0.6:5060");
            Console.ReadKey();
        }
    }
}
