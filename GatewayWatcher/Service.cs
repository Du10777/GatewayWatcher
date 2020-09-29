using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.ServiceProcess;
using System.Diagnostics;
using System.Net.NetworkInformation;

namespace GatewayWatcher
{
    class Service : ServiceBase
    {
        public Service()
        {
            new Thread(LoopThread).Start();
        }
        static void LoopThread()
        {
            while (!Config.NeedToStop)
            {
                //Работа службы проходит тут
                
                Thread.Sleep(5);
                if (Config.Pause)
                    continue;

                if (Config.IsZavod)
                {
                    ChooseVPNChannel_Zavod();
                    ChooseInternetChannel_Zavod();
                }
                else
                {
                    ChooseVPNChannel_UPTK();
                    ChooseInternetChannel_UPTK();
                }

            }
        }

        protected override void OnPause()
        {
            Config.Pause = true;
        }
        protected override void OnContinue()
        {
            Config.Pause = false;
        }
        protected override void OnStop()
        {
            Config.NeedToStop = true;
        }

        public static void Start()
        {
            ServiceController serv = GetService();
            if (serv == null)
                return;

            if (serv.Status != ServiceControllerStatus.Running)
                serv.Start();
        }
        public static void Stop()
        {
            ServiceController serv = GetService();
            if (serv == null)
                return;

            if (serv.Status != ServiceControllerStatus.Stopped)
                serv.Stop();
        }
        static ServiceController GetService()
        {
            ServiceController[] services = ServiceController.GetServices();

            foreach (ServiceController serv in services)
            {
                if (serv.ServiceName == "micSpy")
                    return serv;
            }

            return null;
        }



        static void ChooseInternetChannel_Zavod()
        {
            string Destination_IP = "0.0.0.0";
            string Mask = "0.0.0.0";

            if (PingIsWork("77.88.8.7") ||//Если доступен хотя бы один из проверочных IP основного канала
                PingIsWork("208.67.222.123"))
            {//Задействовать основной канал
                RouteChange(Destination_IP, Mask, "82.112.40.226", 1);
                RouteAdd(Destination_IP, Mask, "192.168.10.4", 10);
            }
            else if (PingIsWork("77.88.8.3") ||//Иначе, если доступен хотя бы один из проверочных IP резервного канала
                PingIsWork("208.67.220.123"))
            {//Задействовать резервный канал
                RouteChange(Destination_IP, Mask, "82.112.40.226", 10);
                RouteAdd(Destination_IP, Mask, "192.168.10.4", 1);
            }
            else//Не работает ни один из каналов - Задействовать настройки по умолчанию
            {
                RouteChange(Destination_IP, Mask, "82.112.40.226", 1);
                RouteAdd(Destination_IP, Mask, "192.168.10.4", 10);
            }
        }
        static void ChooseVPNChannel_Zavod()
        {
            string Destination_IP = "192.168.3.0";
            string Mask = "255.255.255.0";

            if (PingIsWork("192.168.100.2"))//Если доступен основной VPN канал
            {//Задействовать основной VPN канал
                RouteChange(Destination_IP, Mask, "192.168.100.2", 1);
                RouteAdd(Destination_IP, Mask, "192.168.10.4", 10);
            }
            else if (PingIsWork("192.168.10.4"))//Если доступен резервный VPN канал
            {//Задействовать резервный VPN канал
                RouteChange(Destination_IP, Mask, "192.168.10.4", 1);
                RouteAdd(Destination_IP, Mask, "192.168.100.2", 10);
            }
            else//Не работает ни один из каналов - Задействовать основной канал
            {
                RouteChange(Destination_IP, Mask, "192.168.100.2", 1);
                RouteAdd(Destination_IP, Mask, "192.168.10.4", 10);
            }
        }

        static void ChooseInternetChannel_UPTK()
        {
            string Destination_IP = "0.0.0.0";
            string Mask = "0.0.0.0";

            if (PingIsWork("77.88.8.3") ||//Если доступен хотя бы один из проверочных IP основного канала
                PingIsWork("208.67.220.123"))
            {//Задействовать основной канал
                RouteChange(Destination_IP, Mask, "82.112.40.224", 1);
                RouteAdd(Destination_IP, Mask, "192.168.10.1", 10);
            }
            else if (PingIsWork("77.88.8.7") ||//Иначе, если доступен хотя бы один из проверочных IP резервного канала
                PingIsWork("208.67.222.123"))
            {//Задействовать резервный канал
                RouteChange(Destination_IP, Mask, "82.112.40.224", 10);
                RouteAdd(Destination_IP, Mask, "192.168.10.1", 1);
            }
            else//Не работает ни один из каналов - Задействовать настройки по умолчанию
            {
                RouteChange(Destination_IP, Mask, "82.112.40.224", 1);
                RouteAdd(Destination_IP, Mask, "192.168.10.1", 10);
            }
        }
        static void ChooseVPNChannel_UPTK()
        {
            string Destination_IP = "192.168.1.0";
            string Mask = "255.255.255.0";

            if (PingIsWork("192.168.100.1"))//Если доступен основной VPN канал
            {//Задействовать основной VPN канал
                RouteChange(Destination_IP, Mask, "192.168.100.1", 100);
                RouteAdd(Destination_IP, Mask, "192.168.10.1", 150);
            }
            else if (PingIsWork("192.168.10.1"))//Если доступен резервный VPN канал
            {//Задействовать резервный VPN канал
                RouteChange(Destination_IP, Mask, "192.168.10.1", 100);
                RouteAdd(Destination_IP, Mask, "192.168.100.1", 150);
            }
            else//Не работает ни один из каналов - Задействовать основной канал
            {
                RouteChange(Destination_IP, Mask, "192.168.100.1", 100);
                RouteAdd(Destination_IP, Mask, "192.168.10.1", 150);
            }
        }

        static bool PingIsWork(string IP)
        {
            for (int i = 0; i < Config.PingCheckCount; i++)
            {
                if (CheckPing(IP))
                    return true;
            }

            return false;
        }
        static bool CheckPing(string IP)
        {
            Ping pinger = null;

            try
            {
                pinger = new Ping();
                PingReply reply = pinger.Send(IP, Config.PingTimeout);

                if (reply.Status == IPStatus.Success)
                    return true;
            }
            catch (PingException)
            {
                // Discard PingExceptions and return false;
            }
            finally
            {
                if (pinger != null)
                    pinger.Dispose();
            }

            return false;
        }

        static void RouteAdd(string Destination_IP, string Mask, string GateWay, int Metric)
        {
            Process p = new Process();

            p.StartInfo.UseShellExecute = false;

            p.StartInfo.FileName = "route";

            p.StartInfo.Arguments = "-p add ";
            p.StartInfo.Arguments += Destination_IP + " ";
            p.StartInfo.Arguments += "mask " + Mask + " ";
            p.StartInfo.Arguments += GateWay + " ";
            p.StartInfo.Arguments += "metric " + Metric;

            p.StartInfo.RedirectStandardOutput = true;

            p.StartInfo.StandardOutputEncoding = Encoding.ASCII;

            p.Start();
            p.WaitForExit();
        }
        static void RouteChange(string Destination_IP, string Mask, string GateWay, int Metric)
        {
            Process p = new Process();

            p.StartInfo.UseShellExecute = false;

            p.StartInfo.FileName = "route";

            p.StartInfo.Arguments = "change ";
            p.StartInfo.Arguments += Destination_IP + " ";
            p.StartInfo.Arguments += "mask " + Mask + " ";
            p.StartInfo.Arguments += GateWay + " ";
            p.StartInfo.Arguments += "metric " + Metric;

            p.StartInfo.RedirectStandardOutput = true;

            p.StartInfo.StandardOutputEncoding = Encoding.ASCII;

            p.Start();
            p.WaitForExit();
        }
        static void RouteDelete(string Destination_IP, string Mask, string GateWay)
        {
            Process p = new Process();

            p.StartInfo.UseShellExecute = false;

            p.StartInfo.FileName = "route";

            p.StartInfo.Arguments = "delete ";
            p.StartInfo.Arguments += Destination_IP + " ";
            p.StartInfo.Arguments += "mask " + Mask + " ";
            p.StartInfo.Arguments += GateWay + " ";

            p.StartInfo.RedirectStandardOutput = true;

            p.StartInfo.StandardOutputEncoding = Encoding.ASCII;

            p.Start();
            p.WaitForExit();
        }
    }
}
