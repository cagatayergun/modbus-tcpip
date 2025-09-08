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
        private const string ADIM_NO = "3000"; // D3568
        private const string RECETE_MODU = "0"; // Kx30D -> D30.0 -> coil
        private const string MANUEL_MODU = "3"; // Kx30D -> D30.0 -> coil
        private const string PAUSE_DURUMU = "1"; // MX1015 -> M1015
        private const string ALARM_NO = "3001"; // D3604
        private const string ANLIK_SU_SEVIYESI = "3002"; // K200 -> D200
        private const string ANLIK_DEVIR = "3003"; // D6007
        private const string ANLIK_SICAKLIK = "3004"; // D4980
        private const string PROSES_YUZDESI = "3005"; // D7752
        private const string MAKINE_TIPI = "3006"; // D6100
        private const string SIPARIS_NO = "3016"; // D6110
        private const string MUSTERI_NO = "3026"; // D6120
        private const string BATCH_NO = "3036"; // D6130
        private const string OPERATOR_ISMI = "3056"; // D6460
        private const string RECETE_ADI = "3071"; // D2550
        private const string SU_MIKTARI = "3077"; // D7702
        private const string ELEKTRIK_HARCAMA = "3078"; // D7720
        private const string BUHAR_HARCAMA = "3079"; // D7744
        private const string CALISMA_SURESI = "3080"; // D7750
        private const string AKTIF_CALISMA = "2"; // MX2501 -> M2501
        private const string TOPLAM_DURUS_SURESI_SN = "3081"; // D7764 (Int32 için 2 word okunur)
                                                              // private const string STANDART_CEVRIM_SURESI_DK = "3082"; // D6411
        private const string TOPLAM_URETIM_ADEDI = "3082"; // D7768
        private const string HATALI_URETIM_ADEDI = "3083"; // D7770
        private const string ActualQuantity = "3084"; // D7790
        private const string AKTIF_ADIM_TIPI_WORDU = "3085"; // D94
        private const string RECETE_VERI_ADRESI = "3086"; // D100
        private const string OPERATOR_SABLONU_ADRESI = "3087"; // D7500


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
                bool anyReadFailed = false;
                var adimTipiResult = _plcClient.ReadInt16(AKTIF_ADIM_TIPI_WORDU);
                if (adimTipiResult.IsSuccess) status.AktifAdimTipiWordu = adimTipiResult.Content;
                else return OperateResult.CreateFailedResult<FullMachineStatus>(adimTipiResult);

              

                var receteModuResult = _plcClient.ReadCoil(RECETE_MODU);
                if (!receteModuResult.IsSuccess) return OperateResult.CreateFailedResult<FullMachineStatus>(receteModuResult);
                status.IsInRecipeMode = receteModuResult.Content;



                var pauseResult = _plcClient.ReadCoil(PAUSE_DURUMU);
                if (pauseResult.IsSuccess) status.IsPaused = pauseResult.Content;
                else { Debug.WriteLine($"[HATA] {IpAddress} - {PAUSE_DURUMU} (Pause Durumu) okunamadı: {pauseResult.Message}"); anyReadFailed = true; }

                var alarmNoResult = _plcClient.ReadInt16(ALARM_NO);
                if (alarmNoResult.IsSuccess) { status.ActiveAlarmNumber = alarmNoResult.Content+25; status.HasActiveAlarm = alarmNoResult.Content > 25; }
                else { Debug.WriteLine($"[HATA] {IpAddress} - {ALARM_NO} (Alarm No) okunamadı: {alarmNoResult.Message}"); anyReadFailed = true; }

       

                var devirResult = _plcClient.ReadInt16(ANLIK_DEVIR);
                if (devirResult.IsSuccess) status.AnlikDevirRpm = devirResult.Content;
                else { Debug.WriteLine($"[HATA] {IpAddress} - {ANLIK_DEVIR} (Anlık Devir) okunamadı: {devirResult.Message}"); anyReadFailed = true; }

                var sicaklikResult = _plcClient.ReadInt16(ANLIK_SICAKLIK);
                if (sicaklikResult.IsSuccess) status.AnlikSicaklik = sicaklikResult.Content;
                else { Debug.WriteLine($"[HATA] {IpAddress} - {ANLIK_SICAKLIK} (Anlık Sıcaklık) okunamadı: {sicaklikResult.Message}"); anyReadFailed = true; }

                // var yuzdeResult = _plcClient.ReadInt16(PROSES_YUZDESI);
                // if (yuzdeResult.IsSuccess) status.ProsesYuzdesi = yuzdeResult.Content;
                // else { Debug.WriteLine($"[HATA] {IpAddress} - {PROSES_YUZDESI} (Proses Yüzdesi) okunamadı: {yuzdeResult.Message}"); anyReadFailed = true; }

                var operatorResult = ReadStringFromWords(OPERATOR_ISMI, 5);
                if (operatorResult.IsSuccess) status.OperatorIsmi = operatorResult.Content;
                else { Debug.WriteLine($"[HATA] {IpAddress} - {OPERATOR_ISMI} (Operatör İsmi) okunamadı: {operatorResult.Message}"); anyReadFailed = true; }

                var recipeNameResult = ReadStringFromWords(RECETE_ADI, 5);
                if (recipeNameResult.IsSuccess) status.RecipeName = recipeNameResult.Content;
                else { Debug.WriteLine($"[HATA] {IpAddress} - {RECETE_ADI} (Reçete Adı) okunamadı: {recipeNameResult.Message}"); anyReadFailed = true; }

                var siparisNoResult = ReadStringFromWords(SIPARIS_NO, 5);
                if (siparisNoResult.IsSuccess) status.SiparisNumarasi = siparisNoResult.Content;
                else { Debug.WriteLine($"[HATA] {IpAddress} - {SIPARIS_NO} (Sipariş No) okunamadı: {siparisNoResult.Message}"); anyReadFailed = true; }

                var musteriNoResult = ReadStringFromWords(MUSTERI_NO, 5);
                if (musteriNoResult.IsSuccess) status.MusteriNumarasi = musteriNoResult.Content;
                else { Debug.WriteLine($"[HATA] {IpAddress} - {MUSTERI_NO} (Müşteri No) okunamadı: {musteriNoResult.Message}"); anyReadFailed = true; }

                var batchNoResult = ReadStringFromWords(BATCH_NO, 5);
                if (batchNoResult.IsSuccess) status.BatchNumarasi = batchNoResult.Content;
                else { Debug.WriteLine($"[HATA] {IpAddress} - {BATCH_NO} (Batch No) okunamadı: {batchNoResult.Message}"); anyReadFailed = true; }

               

                var elektrikResult = _plcClient.ReadInt16(ELEKTRIK_HARCAMA);
                if (!elektrikResult.IsSuccess) return OperateResult.CreateFailedResult<FullMachineStatus>(elektrikResult);
                status.ElektrikHarcama = elektrikResult.Content;

                var buharResult = _plcClient.ReadInt16(BUHAR_HARCAMA);
                if (!buharResult.IsSuccess) return OperateResult.CreateFailedResult<FullMachineStatus>(buharResult);
                status.BuharHarcama = buharResult.Content;

                var runTimeResult = _plcClient.ReadInt16(CALISMA_SURESI);
                if (!runTimeResult.IsSuccess) return OperateResult.CreateFailedResult<FullMachineStatus>(runTimeResult);
                status.CalismaSuresiDakika = runTimeResult.Content;

                var isProductionResult = _plcClient.ReadCoil(AKTIF_CALISMA);
                if (!isProductionResult.IsSuccess) return OperateResult.CreateFailedResult<FullMachineStatus>(isProductionResult);
                status.IsMachineInProduction = isProductionResult.Content;

                var downTimeResult = _plcClient.ReadInt32(TOPLAM_DURUS_SURESI_SN);
                if (!downTimeResult.IsSuccess) return OperateResult.CreateFailedResult<FullMachineStatus>(downTimeResult);
                status.TotalDownTimeSeconds = downTimeResult.Content;



                var totalProdResult = _plcClient.ReadInt16(TOPLAM_URETIM_ADEDI);
                if (!totalProdResult.IsSuccess) return OperateResult.CreateFailedResult<FullMachineStatus>(totalProdResult);
                status.TotalProductionCount = totalProdResult.Content;

                var defectiveProdResult = _plcClient.ReadInt16(HATALI_URETIM_ADEDI);
                if (!defectiveProdResult.IsSuccess) return OperateResult.CreateFailedResult<FullMachineStatus>(defectiveProdResult);
                status.DefectiveProductionCount = defectiveProdResult.Content;

                var readActualQuantity = _plcClient.ReadInt16(ActualQuantity);
                if (!readActualQuantity.IsSuccess) return OperateResult.CreateFailedResult<FullMachineStatus>(readActualQuantity);
                status.ActualQuantityProduction = readActualQuantity.Content;

              

                var manuel_stat = _plcClient.ReadBool(MANUEL_MODU); // k30c
                if (!manuel_stat.IsSuccess) return OperateResult.CreateFailedResult<FullMachineStatus>(manuel_stat);
                status.manuel_status = manuel_stat.Content;

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

        public async Task<OperateResult<List<PlcOperator>>> ReadPlcOperatorsAsync()
        {
            // DEĞİŞİKLİK: Modbus adres kullanılıyor
            var readResult = await Task.Run(() => _plcClient.ReadInt16(OPERATOR_SABLONU_ADRESI, 120));
            if (!readResult.IsSuccess)
            {
                return OperateResult.CreateFailedResult<List<PlcOperator>>(readResult);
            }

            var operators = new List<PlcOperator>();
            var rawData = readResult.Content;

            for (int i = 0; i < 5; i++)
            {
                int offset = i * 12;

                short[] nameWords = new short[10];
                Array.Copy(rawData, offset, nameWords, 0, 10);
                byte[] nameBytes = new byte[20];
                Buffer.BlockCopy(nameWords, 0, nameBytes, 0, 20);
                string name = Encoding.ASCII.GetString(nameBytes).Trim('\0', ' ');

                operators.Add(new PlcOperator
                {
                    SlotIndex = i,
                    Name = name,
                    UserId = rawData[offset + 10],
                    Password = rawData[offset + 11]
                });
            }

            return OperateResult.CreateSuccessResult(operators);
        }

        public async Task<OperateResult> WritePlcOperatorAsync(PlcOperator plcOperator)
        {
            var operator_write = await Task.Run(() => _plcClient.Write("3210", 1));
            if (operator_write.IsSuccess) ;
            // DEĞİŞİKLİK: Modbus adres kullanılıyor
            string startAddress = (3087 + plcOperator.SlotIndex * 12).ToString();
            byte[] dataToWrite1 = new byte[24];
            byte[] nameBytes1 = Encoding.ASCII.GetBytes(plcOperator.Name.PadRight(20).Substring(0, 20));


            byte[] dataToWrite = new byte[24];
            //    for (int i = 0; i < 24; i += 2)
            //    {
            //    dataToWrite[i] = dataToWrite1[i + 1];
            //     dataToWrite[i + 1] = dataToWrite1[i];
            //     }
            byte[] nameBytes = new byte[20];
            for (int i = 0; i < 20; i += 2)
            {
                nameBytes[i] = nameBytes1[i + 1];
                nameBytes[i + 1] = nameBytes1[i];
            }


            Buffer.BlockCopy(nameBytes, 0, dataToWrite, 0, 20);
            // Buffer.BlockCopy(dataToWrite, 0, dataToWrite, 0, 24);

            dataToWrite[21] = (byte)(plcOperator.UserId & 0xFF); // Düşük bayt
            dataToWrite[20] = (byte)((plcOperator.UserId >> 8) & 0xFF); // Yüksek bayt

            dataToWrite[23] = (byte)(plcOperator.Password & 0xFF); // Düşük bayt
            dataToWrite[22] = (byte)((plcOperator.Password >> 8) & 0xFF); // Yüksek bayt
                                                                          // BitConverter.GetBytes(plcOperator.UserId).CopyTo(dataToWrite, 20);
                                                                          // BitConverter.GetBytes(plcOperator.Password).CopyTo(dataToWrite, 22);
            var writeResult = await Task.Run(() => _plcClient.Write(startAddress, dataToWrite));
            return writeResult;
        }

        public async Task<OperateResult<PlcOperator>> ReadSinglePlcOperatorAsync(int slotIndex)
        {
            var single_operator_write = await Task.Run(() => _plcClient.Write("3211", 1));
            if (single_operator_write.IsSuccess) ;

            string op_no = slotIndex.ToString();

            var single_operator_no = await Task.Run(() => _plcClient.Write(op_no, 1));
            // DEĞİŞİKLİK: Modbus adres kullanılıyor
            string startAddress = (3087 + slotIndex * 12).ToString();

            var readResult = await Task.Run(() => _plcClient.ReadInt16(startAddress, 12));
            if (!readResult.IsSuccess)
            {
                return OperateResult.CreateFailedResult<PlcOperator>(readResult);
            }

            var rawData = readResult.Content;

            short[] nameWords = new short[10];
            Array.Copy(rawData, 0, nameWords, 0, 10);
            byte[] nameBytes = new byte[20];
            Buffer.BlockCopy(nameWords, 0, nameBytes, 0, 20);
            string name = Encoding.ASCII.GetString(nameBytes).Trim('\0', ' ');

            var plcOperator = new PlcOperator
            {
                SlotIndex = slotIndex,
                Name = name,
                UserId = rawData[10],
                Password = rawData[11]
            };

            return OperateResult.CreateSuccessResult(plcOperator);
        }

        public Task<OperateResult<BatchSummaryData>> ReadBatchSummaryDataAsync()
        {
            throw new NotImplementedException("");
        }

        public Task<OperateResult<List<ChemicalConsumptionData>>> ReadChemicalConsumptionDataAsync()
        {
            throw new NotImplementedException("");
        }

        public Task<OperateResult<List<ProductionStepDetail>>> ReadStepAnalysisDataAsync()
        {
            throw new NotImplementedException("");
        }
    }
}