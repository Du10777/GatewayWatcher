using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.NetworkInformation;
using System.Diagnostics;
using System.ServiceProcess;

namespace GatewayWatcher
{
    class Program
    {
        //Принцип работы программы:
        //Проверить работу основного канала
        //Если работает - выставить метрику так, что бы трафик ходил через основной канал, а резервный был запасным
        //Если не работает - выставить метрику так, что бы трафик ходил через резервный канал, а основной был запасным

        static void Main(string[] args)
        {
            RunService();
        }

        static void RunService()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new Service()
            };
            ServiceBase.Run(ServicesToRun);
        }

    }
}
