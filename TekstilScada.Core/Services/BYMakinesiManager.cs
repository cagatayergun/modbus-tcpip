// Services/BYMakinesiManager.cs
using HslCommunication;
//using HslCommunication.Modbus; // Add HslCommunication.Modbus using for Modbus
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
        // CHANGE: ModbusTcpNet is used instead of LSFastEnet
        private readonly ModbusTcpNet _plcClient;
        public string IpAddress { get; private set; }

        #region Modbus Address Constants (Default Modbus addresses corresponding to Lsis addresses)
        // **IMPORTANT**: You must verify these addresses against your PLC's Modbus map.
        // Assumption: D addresses are converted to Holding Registers, M addresses to Coils.
        private const string STEP_NO = "3000"; // D3568
        private const string RECIPE_MODE = "0"; // Kx30D -> D30.0 -> coil
        private const string MANUAL_MODE = "3"; // Kx30D -> D30.0 -> coil
        private const string PAUSE_STATUS = "1"; // MX1015 -> M1015
        private const string ALARM_NO = "3001"; // D3604
        private const string CURRENT_WATER_LEVEL = "3002"; // K200 -> D200
        private const string CURRENT_RPM = "3003"; // D6007
        private const string CURRENT_TEMPERATURE = "3004"; // D4980
        private const string PROCESS_PERCENTAGE = "3005"; // D7752
        private const string MACHINE_TYPE = "3006"; // D6100
        private const string ORDER_NO = "3016"; // D6110
        private const string CUSTOMER_NO = "3026"; // D6120
        private const string BATCH_NO = "3036"; // D6130
        private const string OPERATOR_NAME = "3056"; // D6460
        private const string RECIPE_NAME = "3071"; // D2550
        private const string WATER_QUANTITY = "3077"; // D7702
        private const string ELECTRICITY_CONSUMPTION = "3078"; // D7720
        private const string STEAM_CONSUMPTION = "3079"; // D7744
        private const string OPERATING_TIME = "3080"; // D7750
        private const string IS_ACTIVE = "2"; // MX2501 -> M2501
        private const string TOTAL_DOWNTIME_SECONDS = "3081"; // D7764 (Read 2 words for Int32)
        // private const string STANDARD_CYCLE_TIME_MIN = "3082"; // D6411
        private const string TOTAL_PRODUCTION_COUNT = "3082"; // D7768
        private const string DEFECTIVE_PRODUCTION_COUNT = "3083"; // D7770
        private const string ACTUAL_QUANTITY = "3084"; // D7790
        private const string ACTIVE_STEP_TYPE_WORD = "3085"; // D94
        private const string RECIPE_DATA_ADDRESS = "3086"; // D100
        private const string OPERATOR_TEMPLATE_ADDRESS = "3087"; // D7500
        #endregion

        public BYMakinesiManager(string ipAddress, int port)
        {
            // CHANGE: A new client was created with the ModbusTcpNet class
            _plcClient = new ModbusTcpNet(ipAddress, port);
            this.IpAddress = ipAddress;
            _plcClient.ReceiveTimeOut = 5000;
        }

        public OperateResult Connect()
        {
            // Debug.WriteLine($"[{DateTime.Now:HH:mm:ss}] {IpAddress} (BY) -> Connection being attempted...");
            var result = _plcClient.ConnectServer();
            if (result.IsSuccess)
            { }
            else
            { }
            return result;
        }

        public OperateResult Disconnect()
        {
            return _plcClient.ConnectClose();
        }

        private OperateResult<string> ReadStringFromWords(string address, ushort wordLength)
        {
            // CHANGE: Modbus read operation is used
            var readResult = _plcClient.ReadInt16(address, wordLength);
            if (!readResult.IsSuccess)
            {
                return OperateResult.CreateFailedResult<string>(new OperateResult($"Could not read address block: {address}, Error: {readResult.Message}"));
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
                return new OperateResult<string>($"Error during string conversion: {ex.Message}");
            }
        }

        public OperateResult<FullMachineStatus> ReadLiveStatusData()
        {
            var errorMessages = new List<string>();
            try
            {
                var status = new FullMachineStatus();
                bool anyReadFailed = false;

                // CHANGE: Modbus read operations
                var adimTipiResult = _plcClient.ReadInt16(ACTIVE_STEP_TYPE_WORD);
                if (adimTipiResult.IsSuccess) status.AktifAdimTipiWordu = adimTipiResult.Content;
                else return OperateResult.CreateFailedResult<FullMachineStatus>(adimTipiResult);

                var adimNoResult = _plcClient.ReadInt16(STEP_NO);
                if (adimNoResult.IsSuccess) status.AktifAdimNo = adimNoResult.Content;
                else { Debug.WriteLine($"[ERROR] {IpAddress} - {STEP_NO} (Step No) could not be read: {adimNoResult.Message}"); anyReadFailed = true; }

                var receteModuResult = _plcClient.ReadCoil(RECIPE_MODE);
                if (!receteModuResult.IsSuccess) return OperateResult.CreateFailedResult<FullMachineStatus>(receteModuResult);
                status.IsInRecipeMode = receteModuResult.Content;


                var pauseResult = _plcClient.ReadCoil(PAUSE_STATUS);
                if (pauseResult.IsSuccess) status.IsPaused = pauseResult.Content;
                else { Debug.WriteLine($"[ERROR] {IpAddress} - {PAUSE_STATUS} (Pause Status) could not be read: {pauseResult.Message}"); anyReadFailed = true; }

                var alarmNoResult = _plcClient.ReadInt16(ALARM_NO);
                if (alarmNoResult.IsSuccess) { status.ActiveAlarmNumber = alarmNoResult.Content; status.HasActiveAlarm = alarmNoResult.Content > 0; }
                else { Debug.WriteLine($"[ERROR] {IpAddress} - {ALARM_NO} (Alarm No) could not be read: {alarmNoResult.Message}"); anyReadFailed = true; }

                var suSeviyesiResult = _plcClient.ReadInt16(CURRENT_WATER_LEVEL);
                if (suSeviyesiResult.IsSuccess) status.AnlikSuSeviyesi = suSeviyesiResult.Content;
                else { Debug.WriteLine($"[ERROR] {IpAddress} - {CURRENT_WATER_LEVEL} (Current Water Level) could not be read: {suSeviyesiResult.Message}"); anyReadFailed = true; }

                var devirResult = _plcClient.ReadInt16(CURRENT_RPM);
                if (devirResult.IsSuccess) status.AnlikDevirRpm = devirResult.Content;
                else { Debug.WriteLine($"[ERROR] {IpAddress} - {CURRENT_RPM} (Current RPM) could not be read: {devirResult.Message}"); anyReadFailed = true; }

                var sicaklikResult = _plcClient.ReadInt16(CURRENT_TEMPERATURE);
                if (sicaklikResult.IsSuccess) status.AnlikSicaklik = sicaklikResult.Content;
                else { Debug.WriteLine($"[ERROR] {IpAddress} - {CURRENT_TEMPERATURE} (Current Temperature) could not be read: {sicaklikResult.Message}"); anyReadFailed = true; }

                // var yuzdeResult = _plcClient.ReadInt16(PROCESS_PERCENTAGE);
                // if (yuzdeResult.IsSuccess) status.ProsesYuzdesi = yuzdeResult.Content;
                // else { Debug.WriteLine($"[ERROR] {IpAddress} - {PROCESS_PERCENTAGE} (Process Percentage) could not be read: {yuzdeResult.Message}"); anyReadFailed = true; }

                var operatorResult = ReadStringFromWords(OPERATOR_NAME, 5);
                if (operatorResult.IsSuccess) status.OperatorIsmi = operatorResult.Content;
                else { Debug.WriteLine($"[ERROR] {IpAddress} - {OPERATOR_NAME} (Operator Name) could not be read: {operatorResult.Message}"); anyReadFailed = true; }

                var recipeNameResult = ReadStringFromWords(RECIPE_NAME, 5);
                if (recipeNameResult.IsSuccess) status.RecipeName = recipeNameResult.Content;
                else { Debug.WriteLine($"[ERROR] {IpAddress} - {RECIPE_NAME} (Recipe Name) could not be read: {recipeNameResult.Message}"); anyReadFailed = true; }

                var siparisNoResult = ReadStringFromWords(ORDER_NO, 5);
                if (siparisNoResult.IsSuccess) status.SiparisNumarasi = siparisNoResult.Content;
                else { Debug.WriteLine($"[ERROR] {IpAddress} - {ORDER_NO} (Order No) could not be read: {siparisNoResult.Message}"); anyReadFailed = true; }

                var musteriNoResult = ReadStringFromWords(CUSTOMER_NO, 5);
                if (musteriNoResult.IsSuccess) status.MusteriNumarasi = musteriNoResult.Content;
                else { Debug.WriteLine($"[ERROR] {IpAddress} - {CUSTOMER_NO} (Customer No) could not be read: {musteriNoResult.Message}"); anyReadFailed = true; }

                var batchNoResult = ReadStringFromWords(BATCH_NO, 5);
                if (batchNoResult.IsSuccess) status.BatchNumarasi = batchNoResult.Content;
                else { Debug.WriteLine($"[ERROR] {IpAddress} - {BATCH_NO} (Batch No) could not be read: {batchNoResult.Message}"); anyReadFailed = true; }

                var suResult = _plcClient.ReadInt16(WATER_QUANTITY);
                if (!suResult.IsSuccess) return OperateResult.CreateFailedResult<FullMachineStatus>(suResult);
                status.SuMiktari = suResult.Content;

                var elektrikResult = _plcClient.ReadInt16(ELECTRICITY_CONSUMPTION);
                if (!elektrikResult.IsSuccess) return OperateResult.CreateFailedResult<FullMachineStatus>(elektrikResult);
                status.ElektrikHarcama = elektrikResult.Content;

                var buharResult = _plcClient.ReadInt16(STEAM_CONSUMPTION);
                if (!buharResult.IsSuccess) return OperateResult.CreateFailedResult<FullMachineStatus>(buharResult);
                status.BuharHarcama = buharResult.Content;

                var runTimeResult = _plcClient.ReadInt16(OPERATING_TIME);
                if (!runTimeResult.IsSuccess) return OperateResult.CreateFailedResult<FullMachineStatus>(runTimeResult);
                status.CalismaSuresiDakika = runTimeResult.Content;

                var isProductionResult = _plcClient.ReadCoil(IS_ACTIVE);
                if (!isProductionResult.IsSuccess) return OperateResult.CreateFailedResult<FullMachineStatus>(isProductionResult);
                status.IsMachineInProduction = isProductionResult.Content;

                var downTimeResult = _plcClient.ReadInt32(TOTAL_DOWNTIME_SECONDS);
                if (!downTimeResult.IsSuccess) return OperateResult.CreateFailedResult<FullMachineStatus>(downTimeResult);
                status.TotalDownTimeSeconds = downTimeResult.Content;


                var totalProdResult = _plcClient.ReadInt16(TOTAL_PRODUCTION_COUNT);
                if (!totalProdResult.IsSuccess) return OperateResult.CreateFailedResult<FullMachineStatus>(totalProdResult);
                status.TotalProductionCount = totalProdResult.Content;

                var defectiveProdResult = _plcClient.ReadInt16(DEFECTIVE_PRODUCTION_COUNT);
                if (!defectiveProdResult.IsSuccess) return OperateResult.CreateFailedResult<FullMachineStatus>(defectiveProdResult);
                status.DefectiveProductionCount = defectiveProdResult.Content;

                var readActualQuantity = _plcClient.ReadInt16(ACTUAL_QUANTITY);
                if (!readActualQuantity.IsSuccess) return OperateResult.CreateFailedResult<FullMachineStatus>(readActualQuantity);
                status.ActualQuantityProduction = readActualQuantity.Content;

                var stepDataResult = _plcClient.ReadInt16("70", 25); // D70
                if (!stepDataResult.IsSuccess) return OperateResult.CreateFailedResult<FullMachineStatus>(stepDataResult);
                status.AktifAdimDataWords = stepDataResult.Content;

                var manuel_stat = _plcClient.ReadBool(MANUAL_MODE); // k30c
                if (!manuel_stat.IsSuccess) return OperateResult.CreateFailedResult<FullMachineStatus>(manuel_stat);
                status.manuel_status = manuel_stat.Content;


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
                    Debug.WriteLine($"[PLC READ ERROR] {IpAddress}:\n{combinedErrors}");
                    return new OperateResult<FullMachineStatus>($"Error reading from PLC: {combinedErrors}");
                }
                status.ConnectionState = ConnectionStatus.Connected;
                return OperateResult.CreateSuccessResult(status);
            }
            catch (Exception ex)
            {
                return new OperateResult<FullMachineStatus>($"An exception occurred during read operation: {ex.Message}");
            }
        }

        public Task<OperateResult> AcknowledgeAlarm()
        {
            throw new NotImplementedException("Alarm acknowledgment is not yet implemented for BYMakinesi.");
        }

        public async Task<OperateResult> WriteRecipeToPlcAsync(ScadaRecipe recipe, int? recipeSlot = null)
        {
            // var recipe_write = 1;
            var recipe_write = await Task.Run(() => _plcClient.Write("3209", 1));
            if (recipe_write.IsSuccess)

                if (recipe.Steps.Count != 98) return new OperateResult("Recipe must have 98 steps.");

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
                // CHANGE: Modbus address calculation
                string currentAddress = (100 + i).ToString(); // D100
                ushort readLength = (ushort)Math.Min(chunkSize, fullRecipeData.Length - i);

                var writeResult = await Task.Run(() => _plcClient.Write(currentAddress, fullRecipeData.Skip(i).Take(readLength).ToArray()));
                if (!writeResult.IsSuccess)
                {
                    return new OperateResult($"Recipe write error. Address: {currentAddress}, Error: {writeResult.Message}");
                }
            }

            byte[] recipeNameBytes = Encoding.ASCII.GetBytes(recipe.RecipeName.PadRight(10, ' ').Substring(0, 10));
            // CHANGE: Modbus address is used
            var nameWriteResult = await Task.Run(() => _plcClient.Write("2550", recipeNameBytes));
            if (!nameWriteResult.IsSuccess)
            {
                return new OperateResult($"Recipe name write error: {nameWriteResult.Message}");
            }

            return OperateResult.CreateSuccessResult();
        }

        public async Task<OperateResult<short[]>> ReadRecipeFromPlcAsync()
        {
            short[] fullRecipeData = new short[2450];
            ushort chunkSize = 60;

            for (int i = 0; i < fullRecipeData.Length; i += chunkSize)
            {
                // CHANGE: Modbus address calculation
                string currentAddress = (100 + i).ToString(); // D100
                ushort readLength = (ushort)Math.Min(chunkSize, fullRecipeData.Length - i);

                var readResult = await Task.Run(() => _plcClient.ReadInt16(currentAddress, readLength));
                if (!readResult.IsSuccess)
                {
                    return OperateResult.CreateFailedResult<short[]>(new OperateResult($"Error while reading recipe. Address: {currentAddress}, Error: {readResult.Message}"));
                }

                int lengthToCopy = Math.Min(readLength, readResult.Content.Length);
                Array.Copy(readResult.Content, 0, fullRecipeData, i, lengthToCopy);

                await Task.Delay(20);
            }

            return OperateResult.CreateSuccessResult(fullRecipeData);
        }
        public async Task<OperateResult<ScadaRecipe>> ReadFullRecipeDataAsync()
        {
            var readResult = await ReadRecipeFromPlcAsync(); // Call your own method
            if (!readResult.IsSuccess)
            {
                return OperateResult.CreateFailedResult<ScadaRecipe>(readResult);
            }

            var recipeData = readResult.Content;
            var recipe = new ScadaRecipe
            {
                Steps = new List<ScadaRecipeStep>()
            };

            const int wordsPerStep = 25; // Assumption of 25 words per step
            int totalSteps = recipeData.Length / wordsPerStep;

            for (int i = 0; i < totalSteps; i++)
            {
                var stepWords = new short[wordsPerStep];
                Array.Copy(recipeData, i * wordsPerStep, stepWords, 0, wordsPerStep);

                // Pull step number and other data from PLC data
                var step = new ScadaRecipeStep
                {
                    StepNumber = i + 1, // Step number
                    StepDataWords = stepWords
                };
                recipe.Steps.Add(step);
            }

            return OperateResult.CreateSuccessResult(recipe);
        }
        public async Task<OperateResult<Dictionary<int, string>>> ReadRecipeNamesFromPlcAsync()
        {
            var recipeNames = new Dictionary<int, string>();
            try
            {
                // Recipe names are between D3212-D3812, each name is 6 words (12 bytes)
                const int startAddress = 3212;
                const int wordsPerName = 6;
                const int numRecipes = 99;
                const int totalWords = numRecipes * wordsPerName;

                var readResult = await Task.Run(() => _plcClient.ReadInt16(startAddress.ToString(), (ushort)totalWords));
                await Task.Delay(1000);
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
                    string name = Encoding.ASCII.GetString(nameBytes).Trim('\uFFFD', ' ');

                    if (!string.IsNullOrEmpty(name))
                    {
                        recipeNames.Add(i + 1, name);
                    }
                }
                return OperateResult.CreateSuccessResult(recipeNames);
            }
            catch (Exception ex)
            {
                return new OperateResult<Dictionary<int, string>>($"Error while reading recipe names: {ex.Message}");
            }
        }
        // BYMakinesiManager.cs or KurutmaMakinesiManager.cs
        // ...
        private string ConvertTurkishCharactersToAscii(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            return text
                .Replace("ç", "c").Replace("Ç", "C")
                .Replace("ğ", "g").Replace("Ğ", "G")
                .Replace("ı", "i").Replace("İ", "I")
                .Replace("ö", "o").Replace("Ö", "O")
                .Replace("ş", "s").Replace("Ş", "S")
                .Replace("ü", "u").Replace("Ü", "U");
        }
        // ...
        public async Task<OperateResult> WriteRecipeNameAsync(int recipeNumber, string recipeName)
        {
            try
            {
                const int startAddress = 3212;
                const int wordsPerName = 6;
                const int byteLength = wordsPerName * 2;

                int currentAddress = startAddress + (recipeNumber - 1) * wordsPerName;
                string cleanName = ConvertTurkishCharactersToAscii(recipeName);
                // First, adjust the recipe name to fit within 12 bytes (6 words).
                string paddedName = cleanName.PadRight(byteLength, ' ').Substring(0, byteLength);
                byte[] nameBytes = Encoding.ASCII.GetBytes(paddedName);

                // Swap the bytes in groups of 2.
                byte[] swappedBytes = new byte[byteLength];
                for (int i = 0; i < byteLength; i += 2)
                {
                    swappedBytes[i] = nameBytes[i + 1];
                    swappedBytes[i + 1] = nameBytes[i];
                }

                var writeonay = await Task.Run(() => _plcClient.Write("3813", 1));
                // await Task.Delay(300);
                var writeResult = await Task.Run(() => _plcClient.Write(currentAddress.ToString(), swappedBytes));


                // await Task.Delay(300);
                // var writebitti = await Task.Run(() => _plcClient.Write("3813", 0));


                // await Task.Delay(100);
                return writeResult;
            }
            catch (Exception ex)
            {
                return new OperateResult($"An error occurred while writing the recipe name: {ex.Message}");
            }
        }

        public async Task<OperateResult<List<PlcOperator>>> ReadPlcOperatorsAsync()
        {
            // CHANGE: Modbus address is used
            var readResult = await Task.Run(() => _plcClient.ReadInt16(OPERATOR_TEMPLATE_ADDRESS, 120));
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
            // CHANGE: Modbus address is used
            string startAddress = (3087 + plcOperator.SlotIndex * 12).ToString();
            byte[] dataToWrite1 = new byte[24];
            byte[] nameBytes1 = Encoding.ASCII.GetBytes(plcOperator.Name.PadRight(20).Substring(0, 20));


            byte[] dataToWrite = new byte[24];
            // for (int i = 0; i < 24; i += 2)
            // {
            // dataToWrite[i] = dataToWrite1[i + 1];
            // dataToWrite[i + 1] = dataToWrite1[i];
            // }
            byte[] nameBytes = new byte[20];
            for (int i = 0; i < 20; i += 2)
            {
                nameBytes[i] = nameBytes1[i + 1];
                nameBytes[i + 1] = nameBytes1[i];
            }


            Buffer.BlockCopy(nameBytes, 0, dataToWrite, 0, 20);
            // Buffer.BlockCopy(dataToWrite, 0, dataToWrite, 0, 24);

            dataToWrite[21] = (byte)(plcOperator.UserId & 0xFF); // Low byte
            dataToWrite[20] = (byte)((plcOperator.UserId >> 8) & 0xFF); // High byte

            dataToWrite[23] = (byte)(plcOperator.Password & 0xFF); // Low byte
            dataToWrite[22] = (byte)((plcOperator.Password >> 8) & 0xFF); // High byte
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
            // CHANGE: Modbus address is used
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
                // CHANGE: Modbus address is used
                var waterResult = await Task.Run(() => _plcClient.ReadInt16(WATER_QUANTITY));
                if (!waterResult.IsSuccess) return OperateResult.CreateFailedResult<BatchSummaryData>(waterResult);
                summary.TotalWater = waterResult.Content;
                var electricityResult = await Task.Run(() => _plcClient.ReadInt16(ELECTRICITY_CONSUMPTION));
                if (!electricityResult.IsSuccess) return OperateResult.CreateFailedResult<BatchSummaryData>(electricityResult);
                summary.TotalElectricity = electricityResult.Content;
                var steamResult = await Task.Run(() => _plcClient.ReadInt16(STEAM_CONSUMPTION));
                if (!steamResult.IsSuccess) return OperateResult.CreateFailedResult<BatchSummaryData>(steamResult);
                summary.TotalSteam = steamResult.Content;
                return OperateResult.CreateSuccessResult(summary);
            }
            catch (Exception ex)
            {
                return new OperateResult<BatchSummaryData>($"An exception occurred while reading summary data: {ex.Message}");
            }
        }

        public async Task<OperateResult> ResetOeeCountersAsync()
        {
            // CHANGE: Modbus address is used
            var downTimeResetResult = await Task.Run(() => _plcClient.Write(TOTAL_DOWNTIME_SECONDS, 0));
            if (!downTimeResetResult.IsSuccess)
            {
                return new OperateResult($"Downtime counter could not be reset: {downTimeResetResult.Message}");
            }

            var defectiveResetResult = await Task.Run(() => _plcClient.Write(DEFECTIVE_PRODUCTION_COUNT, 0));
            if (!defectiveResetResult.IsSuccess)
            {
                return new OperateResult($"Defective production counter could not be reset: {defectiveResetResult.Message}");
            }

            return OperateResult.CreateSuccessResult();
        }

        public async Task<OperateResult> IncrementProductionCounterAsync()
        {
            // CHANGE: Modbus address is used
            var readResult = await Task.Run(() => _plcClient.ReadInt16(TOTAL_PRODUCTION_COUNT));
            if (!readResult.IsSuccess)
            {
                return new OperateResult($"Production counter could not be read: {readResult.Message}");
            }

            short newCount = (short)(readResult.Content + 1);
            var writeResult = await Task.Run(() => _plcClient.Write(TOTAL_PRODUCTION_COUNT, newCount));

            return writeResult;
        }
    }
}