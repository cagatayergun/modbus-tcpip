// Services/BYMakinesiManager.cs
using HslCommunication;
//using HslCommunication.Modbus; // Modbus için HslCommunication.Modbus using'ini ekleyin
using HslCommunication.ModBus;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TekstilScada.Models;

namespace TekstilScada.Services
{
    public class BYMakinesiManager : IPlcManager
    {
        // DEĞİŞİKLİK: LSFastEnet yerine ModbusTcpNet kullanılıyor
        private readonly ModbusTcpNet _plcClient;
        public string IpAddress { get; private set; }

        #region Modbus Adres Sabitleri (Lsis adreslerine karşılık gelen varsayılan Modbus adresleri)
        // **ÖNEMLİ**: Bu adresleri PLC'nizin Modbus haritasına göre doğrulamanız gerekir.
        // Varsayım: D adresleri Holding Register'a, M adresleri Coil'e dönüştürüldü.
        private const string ADIM_NO = "3000"; // D3568
        private const string RECETE_MODU = "0"; // Kx30D -> D30.0 -> coil
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
       
        #endregion

        public BYMakinesiManager(string ipAddress, int port)
        {
            // DEĞİŞİKLİK: ModbusTcpNet sınıfı ile yeni bir client oluşturuldu
            _plcClient = new ModbusTcpNet(ipAddress, port);
            this.IpAddress = ipAddress;
            _plcClient.ReceiveTimeOut = 5000;
        }

        public OperateResult Connect()
        {
            Debug.WriteLine($"[{DateTime.Now:HH:mm:ss}] {IpAddress} (BY) -> Bağlantı deneniyor...");
            var result = _plcClient.ConnectServer();
            if (result.IsSuccess)
                Debug.WriteLine($"[{DateTime.Now:HH:mm:ss}] {IpAddress} (BY) -> Bağlantı BAŞARILI.");
            else
                Debug.WriteLine($"[{DateTime.Now:HH:mm:ss}] {IpAddress} (BY) -> Bağlantı BAŞARISIZ: {result.Message}");
            return result;
        }

        public OperateResult Disconnect()
        {
            return _plcClient.ConnectClose();
        }

        private OperateResult<string> ReadStringFromWords(string address, ushort wordLength)
        {
            // DEĞİŞİKLİK: Modbus read operasyonu kullanılıyor
            var readResult = _plcClient.ReadInt16(address, wordLength);
            if (!readResult.IsSuccess)
            {
                return OperateResult.CreateFailedResult<string>(new OperateResult($"Adres bloğu okunamadı: {address}, Hata: {readResult.Message}"));
            }

            try
            {
                byte[] byteData = new byte[readResult.Content.Length * 2];
                Buffer.BlockCopy(readResult.Content, 0, byteData, 0, byteData.Length);
                string value = Encoding.ASCII.GetString(byteData).Trim('\0', ' ');
                return OperateResult.CreateSuccessResult(value);
            }
            catch (Exception ex)
            {
                return new OperateResult<string>($"String dönüşümü sırasında hata: {ex.Message}");
            }
        }

        public OperateResult<FullMachineStatus> ReadLiveStatusData()
        {
            var errorMessages = new List<string>();
            try
            {
                var status = new FullMachineStatus();
                bool anyReadFailed = false;

                // DEĞİŞİKLİK: Modbus read operasyonları
                var adimTipiResult = _plcClient.ReadInt16(AKTIF_ADIM_TIPI_WORDU);
                if (adimTipiResult.IsSuccess) status.AktifAdimTipiWordu = adimTipiResult.Content;
                else return OperateResult.CreateFailedResult<FullMachineStatus>(adimTipiResult);

                var adimNoResult = _plcClient.ReadInt16(ADIM_NO);
                if (adimNoResult.IsSuccess) status.AktifAdimNo = adimNoResult.Content;
                else { Debug.WriteLine($"[HATA] {IpAddress} - {ADIM_NO} (Adım No) okunamadı: {adimNoResult.Message}"); anyReadFailed = true; }

                var receteModuResult = _plcClient.ReadCoil(RECETE_MODU);
                if (!receteModuResult.IsSuccess) return OperateResult.CreateFailedResult<FullMachineStatus>(receteModuResult);
                status.IsInRecipeMode = receteModuResult.Content;

                

                var pauseResult = _plcClient.ReadCoil(PAUSE_DURUMU);
                if (pauseResult.IsSuccess) status.IsPaused = pauseResult.Content;
                else { Debug.WriteLine($"[HATA] {IpAddress} - {PAUSE_DURUMU} (Pause Durumu) okunamadı: {pauseResult.Message}"); anyReadFailed = true; }

                var alarmNoResult = _plcClient.ReadInt16(ALARM_NO);
                if (alarmNoResult.IsSuccess) { status.ActiveAlarmNumber = alarmNoResult.Content; status.HasActiveAlarm = alarmNoResult.Content > 0; }
                else { Debug.WriteLine($"[HATA] {IpAddress} - {ALARM_NO} (Alarm No) okunamadı: {alarmNoResult.Message}"); anyReadFailed = true; }

                var suSeviyesiResult = _plcClient.ReadInt16(ANLIK_SU_SEVIYESI);
                if (suSeviyesiResult.IsSuccess) status.AnlikSuSeviyesi = suSeviyesiResult.Content;
                else { Debug.WriteLine($"[HATA] {IpAddress} - {ANLIK_SU_SEVIYESI} (Anlık Su Seviyesi) okunamadı: {suSeviyesiResult.Message}"); anyReadFailed = true; }

                var devirResult = _plcClient.ReadInt16(ANLIK_DEVIR);
                if (devirResult.IsSuccess) status.AnlikDevirRpm = devirResult.Content;
                else { Debug.WriteLine($"[HATA] {IpAddress} - {ANLIK_DEVIR} (Anlık Devir) okunamadı: {devirResult.Message}"); anyReadFailed = true; }

                var sicaklikResult = _plcClient.ReadInt16(ANLIK_SICAKLIK);
                if (sicaklikResult.IsSuccess) status.AnlikSicaklik = sicaklikResult.Content;
                else { Debug.WriteLine($"[HATA] {IpAddress} - {ANLIK_SICAKLIK} (Anlık Sıcaklık) okunamadı: {sicaklikResult.Message}"); anyReadFailed = true; }

                var yuzdeResult = _plcClient.ReadInt16(PROSES_YUZDESI);
                if (yuzdeResult.IsSuccess) status.ProsesYuzdesi = yuzdeResult.Content;
                else { Debug.WriteLine($"[HATA] {IpAddress} - {PROSES_YUZDESI} (Proses Yüzdesi) okunamadı: {yuzdeResult.Message}"); anyReadFailed = true; }

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

                var suResult = _plcClient.ReadInt16(SU_MIKTARI);
                if (!suResult.IsSuccess) return OperateResult.CreateFailedResult<FullMachineStatus>(suResult);
                status.SuMiktari = suResult.Content;

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

                var stepDataResult = _plcClient.ReadInt16("70", 25); // D70
                if (!stepDataResult.IsSuccess) return OperateResult.CreateFailedResult<FullMachineStatus>(stepDataResult);
                status.AktifAdimDataWords = stepDataResult.Content;

               

                if (adimNoResult.IsSuccess)
                {
                    status.AktifAdimNo = adimNoResult.Content;
                    if (!adimNoResult.IsSuccess)
                    {
                        return OperateResult.CreateFailedResult<FullMachineStatus>(adimNoResult);
                    }
                }

                if (errorMessages.Any())
                {
                    string combinedErrors = string.Join("\n", errorMessages);
                    Debug.WriteLine($"[PLC OKUMA HATASI] {IpAddress}:\n{combinedErrors}");
                    return new OperateResult<FullMachineStatus>($"PLC'den okuma hatası: {combinedErrors}");
                }
                status.ConnectionState = ConnectionStatus.Connected;
                return OperateResult.CreateSuccessResult(status);
            }
            catch (Exception ex)
            {
                return new OperateResult<FullMachineStatus>($"Okuma sırasında istisna oluştu: {ex.Message}");
            }
        }

        public Task<OperateResult> AcknowledgeAlarm()
        {
            throw new NotImplementedException("BYMakinesi için alarm onaylama henüz implemente edilmedi.");
        }

        public async Task<OperateResult> WriteRecipeToPlcAsync(ScadaRecipe recipe, int? recipeSlot = null)
        {
          //  var recipe_write = 1;
            var recipe_write = await Task.Run(() => _plcClient.Write("3209",1));
            if (recipe_write.IsSuccess)
            
            
            if (recipe.Steps.Count != 98) return new OperateResult("Reçete 98 adım olmalıdır.");

            short[] fullRecipeData = new short[2450];
            foreach (var step in recipe.Steps)
            {
                int offset = (step.StepNumber - 1) * 25;
                if (offset + step.StepDataWords.Length <= fullRecipeData.Length)
                {
                    Array.Copy(step.StepDataWords, 0, fullRecipeData, offset, step.StepDataWords.Length);
                }
            }

            ushort chunkSize = 100;
            for (int i = 0; i < fullRecipeData.Length; i += chunkSize)
            {
                // DEĞİŞİKLİK: Modbus adres hesaplaması
                string currentAddress = (100 + i).ToString(); // D100
                short[] chunk = fullRecipeData.Skip(i).Take(chunkSize).ToArray();
                var writeResult = await Task.Run(() => _plcClient.Write(currentAddress, chunk));
                if (!writeResult.IsSuccess)
                {
                    return new OperateResult($"Reçete yazma hatası. Adres: {currentAddress}, Hata: {writeResult.Message}");
                }
            }

            byte[] recipeNameBytes = Encoding.ASCII.GetBytes(recipe.RecipeName.PadRight(10, ' ').Substring(0, 10));
            // DEĞİŞİKLİK: Modbus adres kullanılıyor
            var nameWriteResult = await Task.Run(() => _plcClient.Write("2550", recipeNameBytes));
            if (!nameWriteResult.IsSuccess)
            {
                return new OperateResult($"Reçete ismi yazma hatası: {nameWriteResult.Message}");
            }

            return OperateResult.CreateSuccessResult();
        }

        public async Task<OperateResult<short[]>> ReadRecipeFromPlcAsync()
        {
            short[] fullRecipeData = new short[2450];
            ushort chunkSize = 60;

            for (int i = 0; i < fullRecipeData.Length; i += chunkSize)
            {
                // DEĞİŞİKLİK: Modbus adres hesaplaması
                string currentAddress = (100 + i).ToString(); // D100
                ushort readLength = (ushort)Math.Min(chunkSize, fullRecipeData.Length - i);

                var readResult = await Task.Run(() => _plcClient.ReadInt16(currentAddress, readLength));
                if (!readResult.IsSuccess)
                {
                    return OperateResult.CreateFailedResult<short[]>(new OperateResult($"Reçete okunurken hata oluştu. Adres: {currentAddress}, Hata: {readResult.Message}"));
                }

                int lengthToCopy = Math.Min(readLength, readResult.Content.Length);
                Array.Copy(readResult.Content, 0, fullRecipeData, i, lengthToCopy);

                await Task.Delay(20);
            }

            return OperateResult.CreateSuccessResult(fullRecipeData);
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
            byte[] dataToWrite = new byte[24];
            byte[] nameBytes = Encoding.ASCII.GetBytes(plcOperator.Name.PadRight(20).Substring(0, 20));
            Buffer.BlockCopy(nameBytes, 0, dataToWrite, 0, 20);
            BitConverter.GetBytes(plcOperator.UserId).CopyTo(dataToWrite, 20);
            BitConverter.GetBytes(plcOperator.Password).CopyTo(dataToWrite, 22);
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

        public async Task<OperateResult<BatchSummaryData>> ReadBatchSummaryDataAsync()
        {
            try
            {
                var summary = new BatchSummaryData();
                // DEĞİŞİKLİK: Modbus adres kullanılıyor
                var waterResult = await Task.Run(() => _plcClient.ReadInt16(SU_MIKTARI));
                if (!waterResult.IsSuccess) return OperateResult.CreateFailedResult<BatchSummaryData>(waterResult);
                summary.TotalWater = waterResult.Content;
                var electricityResult = await Task.Run(() => _plcClient.ReadInt16(ELEKTRIK_HARCAMA));
                if (!electricityResult.IsSuccess) return OperateResult.CreateFailedResult<BatchSummaryData>(electricityResult);
                summary.TotalElectricity = electricityResult.Content;
                var steamResult = await Task.Run(() => _plcClient.ReadInt16(BUHAR_HARCAMA));
                if (!steamResult.IsSuccess) return OperateResult.CreateFailedResult<BatchSummaryData>(steamResult);
                summary.TotalSteam = steamResult.Content;
                return OperateResult.CreateSuccessResult(summary);
            }
            catch (Exception ex)
            {
                return new OperateResult<BatchSummaryData>($"Özet verileri okunurken istisna oluştu: {ex.Message}");
            }
        }

       

        
        public async Task<OperateResult> ResetOeeCountersAsync()
        {
            // DEĞİŞİKLİK: Modbus adres kullanılıyor
            var downTimeResetResult = await Task.Run(() => _plcClient.Write(TOPLAM_DURUS_SURESI_SN, 0));
            if (!downTimeResetResult.IsSuccess)
            {
                return new OperateResult($"Duruş süresi sayacı sıfırlanamadı: {downTimeResetResult.Message}");
            }

            var defectiveResetResult = await Task.Run(() => _plcClient.Write(HATALI_URETIM_ADEDI, 0));
            if (!defectiveResetResult.IsSuccess)
            {
                return new OperateResult($"Hatalı üretim sayacı sıfırlanamadı: {defectiveResetResult.Message}");
            }

            return OperateResult.CreateSuccessResult();
        }

        public async Task<OperateResult> IncrementProductionCounterAsync()
        {
            // DEĞİŞİKLİK: Modbus adres kullanılıyor
            var readResult = await Task.Run(() => _plcClient.ReadInt16(TOPLAM_URETIM_ADEDI));
            if (!readResult.IsSuccess)
            {
                return new OperateResult($"Üretim sayacı okunamadı: {readResult.Message}");
            }

            short newCount = (short)(readResult.Content + 1);
            var writeResult = await Task.Run(() => _plcClient.Write(TOPLAM_URETIM_ADEDI, newCount));

            return writeResult;
        }
    }
}