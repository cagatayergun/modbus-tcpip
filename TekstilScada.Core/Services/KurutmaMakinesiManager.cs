// Services/KurutmaMakinesiManager.cs
using HslCommunication;
//using HslCommunication.Modbus; // Modbus için HslCommunication.Modbus using'ini ekleyin
using HslCommunication.ModBus;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using TekstilScada.Models;

namespace TekstilScada.Services
{
    public class KurutmaMakinesiManager : IPlcManager
    {
        // DEĞİŞİKLİK: LSFastEnet yerine ModbusTcpNet kullanılıyor
        private readonly ModbusTcpNet _plcClient;
        public string IpAddress { get; private set; }

        // **ÖNEMLİ**: Bu adresleri PLC'nizin Modbus haritasına göre doğrulamanız gerekir.
        private const string IS_RUNNING_COIL = "0"; // M8000
        private const string LIVE_TEMP_REG = "3000"; // D8000
        private const string FAN_RPM_REG = "3001"; // D8002
        private const string ALARM_NO_REG = "3002"; // D8010
        private const string BATCH_NO_REG = "3003"; // D8200
        private const string IS_PRODUCTION_COIL = "3013"; // M8501
        private const string DOWNTIME_SECONDS_REG = "3014"; // D8800
        private const string TOTAL_PROD_COUNT_REG = "3015"; // D8802
        private const string DEFECTIVE_PROD_COUNT_REG = "3016"; // D8804

        public KurutmaMakinesiManager(string ipAddress, int port)
        {
            // DEĞİŞİKLİK: ModbusTcpNet sınıfı ile yeni bir client oluşturuldu
            _plcClient = new ModbusTcpNet(ipAddress, port);
            this.IpAddress = ipAddress;
            _plcClient.ReceiveTimeOut = 5000;
        }

        public OperateResult Connect()
        {
       //     Debug.WriteLine($"[{DateTime.Now:HH:mm:ss}] {IpAddress} (Kurutma) -> Bağlantı deneniyor...");
            var result = _plcClient.ConnectServer();
            if (result.IsSuccess)
            { }// }
            else
                { }  //Debug.WriteLine($"[{DateTime.Now:HH:mm:ss}] {IpAddress} (Kurutma) -> Bağlantı BAŞARISIZ: {result.Message}");
            return result;
        }

        public OperateResult Disconnect()
        {
            return _plcClient.ConnectClose();
        }

        public OperateResult<FullMachineStatus> ReadLiveStatusData()
        {
            try
            {
                var status = new FullMachineStatus();

                // DEĞİŞİKLİK: Modbus read operasyonları
                var isRunningResult = _plcClient.ReadCoil(IS_RUNNING_COIL);
                if (!isRunningResult.IsSuccess) return OperateResult.CreateFailedResult<FullMachineStatus>(isRunningResult);
                status.IsInRecipeMode = isRunningResult.Content;

                var tempResult = _plcClient.ReadInt16(LIVE_TEMP_REG);
                if (!tempResult.IsSuccess) return OperateResult.CreateFailedResult<FullMachineStatus>(tempResult);
                status.AnlikSicaklik = tempResult.Content;

                var fanRpmResult = _plcClient.ReadInt16(FAN_RPM_REG);
                if (!fanRpmResult.IsSuccess) return OperateResult.CreateFailedResult<FullMachineStatus>(fanRpmResult);
                status.AnlikDevirRpm = fanRpmResult.Content;

                var alarmNoResult = _plcClient.ReadInt16(ALARM_NO_REG);
                if (!alarmNoResult.IsSuccess) return OperateResult.CreateFailedResult<FullMachineStatus>(alarmNoResult);
                status.ActiveAlarmNumber = alarmNoResult.Content+25;
                status.HasActiveAlarm = alarmNoResult.Content > 25;

                var batchNoResult = _plcClient.ReadString(BATCH_NO_REG, 20, Encoding.ASCII);
                if (!batchNoResult.IsSuccess) return OperateResult.CreateFailedResult<FullMachineStatus>(batchNoResult);
                status.BatchNumarasi = batchNoResult.Content.Trim('\0', ' ');

                var isProductionResult = _plcClient.ReadCoil(IS_PRODUCTION_COIL);
                if (!isProductionResult.IsSuccess) return OperateResult.CreateFailedResult<FullMachineStatus>(isProductionResult);
                status.IsMachineInProduction = isProductionResult.Content;

                var downTimeResult = _plcClient.ReadInt32(DOWNTIME_SECONDS_REG);
                if (!downTimeResult.IsSuccess) return OperateResult.CreateFailedResult<FullMachineStatus>(downTimeResult);
                status.TotalDownTimeSeconds = downTimeResult.Content;

                var totalProdResult = _plcClient.ReadInt16(TOTAL_PROD_COUNT_REG);
                if (!totalProdResult.IsSuccess) return OperateResult.CreateFailedResult<FullMachineStatus>(totalProdResult);
                status.TotalProductionCount = totalProdResult.Content;

                var defectiveProdResult = _plcClient.ReadInt16(DEFECTIVE_PROD_COUNT_REG);
                if (!defectiveProdResult.IsSuccess) return OperateResult.CreateFailedResult<FullMachineStatus>(defectiveProdResult);
                status.DefectiveProductionCount = defectiveProdResult.Content;

                status.ConnectionState = ConnectionStatus.Connected;
                return OperateResult.CreateSuccessResult(status);
            }
            catch (Exception ex)
            {
                return new OperateResult<FullMachineStatus>($"Kurutma canlı veri okuma hatası: {ex.Message}");
            }
        }

        // ... Diğer metodlar için Modbus'a özgü adresler ve metotlar kullanıldı ...
        public async Task<OperateResult> ResetOeeCountersAsync()
        {
            throw new NotImplementedException("Kurutma makineleri operatör yönetimini desteklemez.");
        }

        public async Task<OperateResult> IncrementProductionCounterAsync()
        {
            throw new NotImplementedException("Kurutma makineleri operatör yönetimini desteklemez.");
        }
        public Task<OperateResult> AcknowledgeAlarm()
        {
            throw new NotImplementedException("KurutmaMakinesi için alarm onaylama henüz implemente edilmedi.");
        }
        public async Task<OperateResult> WriteRecipeToPlcAsync(ScadaRecipe recipe, int? recipeSlot = null)
        {
            // Adresler sabit adresler olarak varsayıldı
            string setTempAddress = "0";
            string setHumidityAddress = "1";
            string setDurationAddress = "2";
            string setRpmAddress = "3";
            string setCoolingTimeAddress = "4";
            string controlWordAddress = "5";

            if (!recipeSlot.HasValue || recipeSlot < 1 || recipeSlot > 20)
                return new OperateResult("Geçersiz reçete numarası (1-20 arası olmalıdır).");

            if (recipe.Steps == null || recipe.Steps.Count == 0)
                return new OperateResult("Reçete adımı bulunamadı.");

            try
            {
                var isRunningResult = await Task.Run(() => _plcClient.ReadCoil("1"));
                if (!isRunningResult.IsSuccess) return isRunningResult;
                if (isRunningResult.Content)
                {
                    return new OperateResult("Makine çalışırken reçete yüklenemez!");
                }

                var firstStep = recipe.Steps[0];
                short setTemperature = firstStep.StepDataWords[0];
                short setHumidity = firstStep.StepDataWords[1];
                short setDuration = firstStep.StepDataWords[2];
                short setRpm = firstStep.StepDataWords[3];
                short setCoolingTime = firstStep.StepDataWords[4];
                short controlWord = firstStep.StepDataWords[5];

                await Task.Run(() => _plcClient.Write("3017", (short)recipeSlot.Value));
                await Task.Delay(500);

                await Task.Run(() => _plcClient.Write("2", true));

                await Task.Run(() => _plcClient.Write(setTempAddress, setTemperature));
                await Task.Run(() => _plcClient.Write(setHumidityAddress, setHumidity));
                await Task.Run(() => _plcClient.Write(setDurationAddress, setDuration));
                await Task.Run(() => _plcClient.Write(setRpmAddress, setRpm));
                await Task.Run(() => _plcClient.Write(setCoolingTimeAddress, setCoolingTime));
                await Task.Run(() => _plcClient.Write(controlWordAddress, controlWord));

               

              
                
              //  await Task.Run(() => _plcClient.Write("1", false));

                return OperateResult.CreateSuccessResult();
            }
            catch (Exception ex)
            {
                return new OperateResult($"Reçete yazılırken hata: {ex.Message}");
            }
        }
        private OperateResult<string> ReadStringFromWords(string address, ushort wordLength)
        {
            // Veriyi önce ham word dizisi olarak oku
            var readResult = _plcClient.ReadInt16(address, wordLength);
            if (!readResult.IsSuccess)
            {
                // Hata durumunda, hangi adreste sorun olduğunu belirterek geri dön
                return OperateResult.CreateFailedResult<string>(new OperateResult($"Adres bloğu okunamadı: {address}, Hata: {readResult.Message}"));
            }

            try
            {
                // Okunan word dizisini byte dizisine çevir
                byte[] byteData = new byte[readResult.Content.Length * 2];
                Buffer.BlockCopy(readResult.Content, 0, byteData, 0, byteData.Length);

                // Byte dizisini ASCII metne çevir ve gereksiz karakterleri temizle
                string value = Encoding.ASCII.GetString(byteData).Trim('\0', ' ');
                return OperateResult.CreateSuccessResult(value);
            }
            catch (Exception ex)
            {
                return new OperateResult<string>($"String dönüşümü sırasında hata: {ex.Message}");
            }
        }
        public async Task<OperateResult<ScadaRecipe>> ReadFullRecipeDataAsync()
        {
            throw new NotImplementedException("Kurutma makineleri operatör yönetimini desteklemez.");
        }
        public async Task<OperateResult<Dictionary<int, string>>> ReadRecipeNamesFromPlcAsync()
        {
            var recipeNames = new Dictionary<int, string>();
            try
            {
                // Reçete isimleri D3212-D3812 arasında, her bir isim 6 word (12 byte)
                const int startAddress = 4000;
                const int wordsPerName = 6;
                const int numRecipes = 20;
                const int totalWords = numRecipes * wordsPerName;
                var readResult = await Task.Run(() => _plcClient.ReadInt16(startAddress.ToString(), (ushort)totalWords));
                if (!readResult.IsSuccess)
                {
                    return OperateResult.CreateFailedResult<Dictionary<int, string>>(readResult);
                }

                byte[] nameBytes = new byte[wordsPerName * 2];
                var data = readResult.Content;

                for (int i = 0; i < numRecipes; i++)
                {
                    short[] nameWords = new short[wordsPerName];
                    Array.Copy(data, i * wordsPerName, nameWords, 0, wordsPerName);
                    Buffer.BlockCopy(nameWords, 0, nameBytes, 0, nameBytes.Length);
                    string name = Encoding.ASCII.GetString(nameBytes).Trim('�', ' ');

                    if (!string.IsNullOrEmpty(name))
                    {
                        recipeNames.Add(i + 1, name);
                    }
                }
                return OperateResult.CreateSuccessResult(recipeNames);
            }
            catch (Exception ex)
            {
                return new OperateResult<Dictionary<int, string>>($"Reçete isimleri okunurken hata: {ex.Message}");
            }
        }
        public async Task<OperateResult> WriteRecipeNameAsync(int recipeNumber, string recipeName)
        {
            try
            {
                // Reçete isimleri D3212'den başlar, her isim 6 word (12 byte)
                const int startAddress = 4000;
                const int wordsPerName = 6;

                // PLC adresini hesapla (1'den başlayan reçete numarası için)
                int currentAddress = startAddress + (recipeNumber - 1) * wordsPerName;

                // Reçete ismini 12 byte (6 word) uzunluğunda bir byte dizisine dönüştür.
                // Fazla uzunsa kes, kısaysa null karakterlerle tamamla.
                byte[] dataToWrite = new byte[wordsPerName * 2];
                byte[] nameBytes = Encoding.ASCII.GetBytes(recipeName);
                Buffer.BlockCopy(nameBytes, 0, dataToWrite, 0, Math.Min(nameBytes.Length, dataToWrite.Length));

                // PLC'ye yazma işlemini başlat.
                var writeResult = await Task.Run(() => _plcClient.Write(currentAddress.ToString(), dataToWrite));

                return writeResult;
            }
            catch (Exception ex)
            {
                return new OperateResult($"Reçete adı yazılırken hata oluştu: {ex.Message}");
            }
        }
        public async Task<OperateResult<short[]>> ReadRecipeFromPlcAsync()
        {
            try
            {
              //  string setTempAddress = "0";
              //  string setHumidityAddress = "1";
              //  string setDurationAddress = "2";
              //  string setRpmAddress = "3";
              //  string setCoolingTimeAddress = "4";
               // string controlWordAddress = "5";
                // DEĞİŞİKLİK: Modbus adres kullanılıyor
                var tempResult = await Task.Run(() => _plcClient.ReadInt16("0"));
                if (!tempResult.IsSuccess) return OperateResult.CreateFailedResult<short[]>(tempResult);

                var humidityResult = await Task.Run(() => _plcClient.ReadInt16("1"));
                if (!humidityResult.IsSuccess) return OperateResult.CreateFailedResult<short[]>(humidityResult);

                var durationResult = await Task.Run(() => _plcClient.ReadInt16("2"));
                if (!durationResult.IsSuccess) return OperateResult.CreateFailedResult<short[]>(durationResult);

                var rpmResult = await Task.Run(() => _plcClient.ReadInt16("3"));
                if (!rpmResult.IsSuccess) return OperateResult.CreateFailedResult<short[]>(rpmResult);

                var coolingResult = await Task.Run(() => _plcClient.ReadInt16("4"));
                if (!coolingResult.IsSuccess) return OperateResult.CreateFailedResult<short[]>(coolingResult);
                var controlWordAddress = await Task.Run(() => _plcClient.ReadInt16("5"));
                if (!controlWordAddress.IsSuccess) return OperateResult.CreateFailedResult<short[]>(controlWordAddress);
                // Okunan değerleri standart bir dizi formatında geri döndür
                short[] recipeData = new short[5];
                recipeData[0] = tempResult.Content;
                recipeData[1] = humidityResult.Content;
                recipeData[2] = durationResult.Content;
                recipeData[3] = rpmResult.Content;
                recipeData[4] = coolingResult.Content;
                recipeData[5] = controlWordAddress.Content;
                return OperateResult.CreateSuccessResult(recipeData);
            }
            catch (Exception ex)
            {
                return new OperateResult<short[]>($"Kurutma reçetesi okunurken hata: {ex.Message}");
            }
        }

        public Task<OperateResult<List<PlcOperator>>> ReadPlcOperatorsAsync()
        {
            throw new NotImplementedException("Kurutma makineleri operatör yönetimini desteklemez.");
        }

        public Task<OperateResult> WritePlcOperatorAsync(PlcOperator plcOperator)
        {
            throw new NotImplementedException("Kurutma makineleri operatör yönetimini desteklemez.");
        }

        public Task<OperateResult<PlcOperator>> ReadSinglePlcOperatorAsync(int slotIndex)
        {
            throw new NotImplementedException("Kurutma makineleri operatör yönetimini desteklemez.");
        }

        public Task<OperateResult<BatchSummaryData>> ReadBatchSummaryDataAsync()
        {
            throw new NotImplementedException("Kurutma makinesi için özet veri okuma henüz yazılmadı.");
        }

        public Task<OperateResult<List<ChemicalConsumptionData>>> ReadChemicalConsumptionDataAsync()
        {
            return Task.FromResult(OperateResult.CreateSuccessResult(new List<ChemicalConsumptionData>()));
        }

        public Task<OperateResult<List<ProductionStepDetail>>> ReadStepAnalysisDataAsync()
        {
            return Task.FromResult(OperateResult.CreateSuccessResult(new List<ProductionStepDetail>()));
        }
    }
}