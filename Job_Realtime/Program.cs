using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Job_Realtime
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                ProcessReailTime();

            }
            catch (Exception ex)
            {
                ShareRules.WriteLog(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
            }
        }


        public static void ProcessReailTime()
        {
            try
            {
                InsertSalesRealTime();
                InsertIncremental();
                //InsertSalesRealTime(GetDatainRabbitMQ(ShareRules.Queue_Sales));


                //InsertSalesRealTime(GetDatainRabbitMQ(ShareRules.Queue_Sales));
                // InsertIncremental(GetDatainRabbitMQ(ShareRules.Queue_Incremental));



            }
            catch (Exception ex)
            {

                ShareRules.WriteLog(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
            }
        }

        public static String GetDatainRabbitMQ(String queueName)
        {
            String Result = String.Empty;

            try
            {
                var factory = new ConnectionFactory() { Uri = new Uri(ShareRules.Amqps_Realtime) };

                factory.UserName = ShareRules.Rabbitmq_User;
                factory.Password = ShareRules.Rabbitmq_Pass;
                using (var connection = factory.CreateConnection())
                {
                    using (var channel = connection.CreateModel())
                    {

                        var data = channel.BasicGet(queueName, false);
                        var message = Encoding.UTF8.GetString(data.Body);
                        channel.BasicAck(data.DeliveryTag, false);
                        if (!string.IsNullOrEmpty(message))
                            return message;


                    }

                }

            }
            catch (Exception ex)
            {
                ShareRules.WriteLog(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
            }
            return Result;

        }

        public static int CountMessageInQuere(String queueName)
        {
            int Total = 0;

            try
            {
                var factory = new ConnectionFactory() { Uri = new Uri(ShareRules.Amqps_Realtime) };

                factory.UserName = ShareRules.Rabbitmq_User;
                factory.Password = ShareRules.Rabbitmq_Pass;
                using (var connection = factory.CreateConnection())
                {
                    using (var channel = connection.CreateModel())
                    {
                        var Result = channel.MessageCount(queueName);
                        if (Result != null) int.TryParse(Result.ToString(), out Total);
                    }
                }
            }
            catch (Exception ex)
            {
                ShareRules.WriteLog(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
            }
            return Total;
        }

        public static void InsertSalesRealTime()
        {
            try
            {
                int totalQuere = 0;
                totalQuere = CountMessageInQuere(ShareRules.Queue_Sales);
                if (totalQuere != 0)
                {
                    for (int row = 0; row < totalQuere; row++)
                    {
                        String message = string.Empty;
                        message = GetDatainRabbitMQ(ShareRules.Queue_Sales);
                        if (!string.IsNullOrEmpty(message))
                        {

                            Sales data = new Sales();
                            data = Newtonsoft.Json.JsonConvert.DeserializeObject<Sales>(message);
                            List<TRN_Sale> ListSaveData = new List<TRN_Sale>();

                            foreach (salesTicket itemsalesTicket in data.salesTicket)
                            {

                                for (int i = 0; i < itemsalesTicket.products.Count; i++)
                                {
                                    TRN_Sale dataSave = new TRN_Sale();
                                    dataSave.CreateDate = DateTime.Now;

                                    if (!ShareRules.CheckEmpty(data.updated))
                                    {
                                        if (i == 0)
                                        {
                                            data.updated = string.Format("{0}-{1}-{2} {3}:{4}:{5}",
                                          "" + data.updated[0] + data.updated[1] + data.updated[2] + data.updated[3],
                                          "" + data.updated[4] + data.updated[5],
                                          "" + data.updated[6] + data.updated[7],
                                           "" + data.updated[8] + data.updated[9],
                                            "" + data.updated[10] + data.updated[11],
                                             "" + data.updated[12] + data.updated[13]);

                                        }
                                        dataSave.UpdateDate = Convert.ToDateTime(data.updated);
                                        //"2019-09-03 13:47:00+0700"

                                    }



                                    if (!ShareRules.CheckEmpty(data.seq))
                                        dataSave.Seq = Convert.ToInt32(data.seq);

                                    dataSave.bu = data.bu;

                                    dataSave.@interface = "today_sales_summary";
                                    dataSave.channel = data.channel;
                                    dataSave.loc = itemsalesTicket.loc;


                                    if (!ShareRules.CheckEmpty(itemsalesTicket.receiptDate))
                                    {
                                        if (i == 0)
                                        {
                                            //"2019-09-03 13:47:00+0700"
                                            itemsalesTicket.receiptDate = string.Format("{0}-{1}-{2} {3}:{4}:{5}",
                                            "" + itemsalesTicket.receiptDate[0] + itemsalesTicket.receiptDate[1] + itemsalesTicket.receiptDate[2] + itemsalesTicket.receiptDate[3],
                                            "" + itemsalesTicket.receiptDate[4] + itemsalesTicket.receiptDate[5],
                                            "" + itemsalesTicket.receiptDate[6] + itemsalesTicket.receiptDate[7],
                                            "" + itemsalesTicket.receiptDate[8] + itemsalesTicket.receiptDate[9],
                                            "" + itemsalesTicket.receiptDate[10] + itemsalesTicket.receiptDate[11],
                                            "" + itemsalesTicket.receiptDate[12] + itemsalesTicket.receiptDate[13]);
                                        }
                                        dataSave.ReceiptDate = Convert.ToDateTime(itemsalesTicket.receiptDate);
                                    }


                                    dataSave.TicketNo = itemsalesTicket.ticketNo;
                                    dataSave.ReceivedNo = itemsalesTicket.receivedNo;
                                    dataSave.CustomerNo = itemsalesTicket.customerNo;
                                    dataSave.CustomerName = itemsalesTicket.customerName;
                                    dataSave.BarCode = itemsalesTicket.products[i].barcode;
                                    dataSave.SKU = itemsalesTicket.products[i].sku;
                                    dataSave.QTY = itemsalesTicket.products[i].qty;
                                    if (itemsalesTicket.payments.Count > i)
                                    {
                                        dataSave.PaymentType = itemsalesTicket.payments[i].paymentType;
                                        dataSave.TenderCode = itemsalesTicket.payments[i].tenderCode;
                                        dataSave.NetSaleAmt = itemsalesTicket.payments[i].netSaleAmt;
                                    }
                                    dataSave.TotalNetSaleAmt = itemsalesTicket.totalNetSaleAmt;
                                    dataSave.TransType = itemsalesTicket.transType;
                                    ListSaveData.Add(dataSave);
                                }
                            }





                            if (ListSaveData.Count > 0)
                            {
                                DBRealTimeDataContext db = ShareRules.ConnectDB(true);
                                db.TRN_Sales.InsertAllOnSubmit(ListSaveData);

                                db.SubmitChanges();
                            }

                        }
                    }
                }


            }
            catch (Exception ex)
            {
                ShareRules.WriteLog(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
            }
        }

        public static void InsertIncremental()
        {
            try
            {
                int totalQuere = 0;
                totalQuere = CountMessageInQuere(ShareRules.Queue_Incremental);
                if (totalQuere != 0)
                {
                    for (int row = 0; row < totalQuere; row++)
                    {
                        String message = string.Empty;
                        message = GetDatainRabbitMQ(ShareRules.Queue_Incremental);
                        if (!string.IsNullOrEmpty(message))
                        {

                            Incremental data = new Incremental();
                            data = Newtonsoft.Json.JsonConvert.DeserializeObject<Incremental>(message);
                            List<TRN_Incremental> ListSaveData = new List<TRN_Incremental>();
                            if (data != null)
                            {
                                foreach (var items in data.items)
                                {
                                    foreach (var itemstocks in items.stocks)
                                    {
                                        TRN_Incremental dataSave = new TRN_Incremental();
                                        dataSave.CreateDate = DateTime.Now;
                                        if (!ShareRules.CheckEmpty(data.updated))
                                            dataSave.UpdateDate = Convert.ToDateTime(data.updated);

                                        if (!ShareRules.CheckEmpty(data.seq))
                                            dataSave.Seq = Convert.ToInt32(data.seq);

                                        dataSave.bu = data.bu;

                                        dataSave.@interface = "online_stock_incre";
                                        dataSave.barcode = items.barcode;
                                        dataSave.SKU = items.sku;
                                        dataSave.Status = items.status;
                                        dataSave.Itemize = items.itemize;
                                        dataSave.Loc = itemstocks.loc;
                                        dataSave.AvailStock = itemstocks.availStock;
                                        dataSave.ReserveStock = itemstocks.reserveStock;
                                        ListSaveData.Add(dataSave);

                                    }
                                }

                                if (ListSaveData.Count > 0)
                                {
                                    DBRealTimeDataContext db = ShareRules.ConnectDB(true);
                                    db.TRN_Incrementals.InsertAllOnSubmit(ListSaveData);

                                    db.SubmitChanges();
                                }


                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShareRules.WriteLog(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
            }
        }
    }
}
