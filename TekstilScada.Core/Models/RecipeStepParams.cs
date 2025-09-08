// Models/RecipeStepParams.cs
using System;
using System.Text;

namespace TekstilScada.Models
{
    /// <summary>
    /// PLC'ye gönderilen 25 word'lük ham veriyi tutan ve ona erişimi kolaylaştıran temel sınıf.
    /// </summary>
    public abstract class BaseStepParams
    {
        protected readonly short[] _data;

        public BaseStepParams(short[] data)
        {
            // Veri dizisinin 25 elemanlı olduğundan emin ol
            if (data == null || data.Length != 25)
            {
                throw new ArgumentException("Parametre veri dizisi 25 elemanlı olmalıdır.", nameof(data));
            }
            _data = data;
        }

        /// <summary>
        /// Belirtilen word'deki belirli bir bit'in durumunu alır veya ayarlar.
        /// </summary>
        protected bool GetBit(int wordIndex, int bitIndex)
        {
            return (_data[wordIndex] & (1 << bitIndex)) != 0;
        }

        protected void SetBit(int wordIndex, int bitIndex, bool value)
        {
            if (value)
            {
                _data[wordIndex] = (short)(_data[wordIndex] | (1 << bitIndex));
            }
            else
            {
                _data[wordIndex] = (short)(_data[wordIndex] & ~(1 << bitIndex));
            }
        }
    }

    #region Adım Parametre Modelleri

    public class SuAlmaParams : BaseStepParams
    {
        public SuAlmaParams(short[] data) : base(data) { }

        // Word 1: Su Litre
        public short MiktarLitre { get => _data[1]; set => _data[1] = value; }

        // Word 0: Kontrol Bitleri
        public bool SicakSu { get => GetBit(0, 0); set => SetBit(0, 0, value); }
        public bool SogukSu { get => GetBit(0, 1); set => SetBit(0, 1, value); }
        public bool YumusakSu { get => GetBit(0, 2); set => SetBit(0, 2, value); }
        public bool TamburDur { get => GetBit(0, 3); set => SetBit(0, 3, value); }
        public bool Alarm { get => GetBit(0, 4); set => SetBit(0, 4, value); }
    }

    public class IsitmaParams : BaseStepParams
    {
        public IsitmaParams(short[] data) : base(data) { }

        // Word 3: Isı (°C)
        public short Isi { get => _data[3]; set => _data[3] = value; }
        // Word 4: Süre (DK)
        public short Sure { get => _data[4]; set => _data[4] = value; }

        // Word 2: Kontrol Bitleri
        public bool DirekBuhar { get => GetBit(2, 0); set => SetBit(2, 0, value); }
        public bool DolayliBuhar { get => GetBit(2, 1); set => SetBit(2, 1, value); }
        public bool TamburDur { get => GetBit(2, 2); set => SetBit(2, 2, value); }
        public bool Alarm { get => GetBit(2, 3); set => SetBit(2, 3, value); }
    }

    public class CalismaParams : BaseStepParams
    {
        public CalismaParams(short[] data) : base(data) { }

        // Word 14: Sağ-Sol Yön Süre (SN)
        public short SagSolSure { get => _data[14]; set => _data[14] = value; }
        // Word 15: Bekleme Süresi (SN)
        public short BeklemeSuresi { get => _data[15]; set => _data[15] = value; }
        // Word 16: Çalışma Devri
        public short CalismaDevri { get => _data[16]; set => _data[16] = value; }
        // Word 18: Çalışma Süresi (DK)
        public short CalismaSuresi { get => _data[18]; set => _data[18] = value; }

        // Word 17: Kontrol Bitleri
        public bool IsiKontrol { get => GetBit(17, 0); set => SetBit(17, 0, value); }
        public bool Alarm { get => GetBit(17, 1); set => SetBit(17, 1, value); }
    }

    public class DozajParams : BaseStepParams
    {
        public DozajParams(short[] data) : base(data) { }

        // Word 21, 22, 23: Kimyasal Adı (6 byte)
        public string Kimyasal
        {
            get
            {
                byte[] kimyasalBytes = new byte[6];
                Buffer.BlockCopy(_data, 21 * 2, kimyasalBytes, 0, 6);
                return Encoding.ASCII.GetString(kimyasalBytes).Trim('\0');
            }
            set
            {
                byte[] kimyasalBytes = Encoding.ASCII.GetBytes(value.PadRight(6).Substring(0, 6));
                Buffer.BlockCopy(kimyasalBytes, 0, _data, 21 * 2, 6);
            }
        }
        // Word 20: Tank Alınan Su
        public short TankSu { get => _data[20]; set => _data[20] = value; }
        // Word 6: Kimyasal Çözme Süresi
        public short CozmeSure { get => _data[6]; set => _data[6] = value; }
        // Word 7: Kimyasal Dozaj Süresi
        public short DozajSure { get => _data[7]; set => _data[7] = value; }
        // Word 11: Dozajda Litre
        public short DozajLitre { get => _data[11]; set => _data[11] = value; }

        // Word 5: Kontrol Bitleri
        public bool AnaTankMakSu { get => GetBit(5, 1); set => SetBit(5, 1, value); }
        public bool AnaTankTemizSu { get => GetBit(5, 2); set => SetBit(5, 2, value); }
        public bool TamburDur { get => GetBit(5, 3); set => SetBit(5, 3, value); }
        public bool Alarm { get => GetBit(5, 4); set => SetBit(5, 4, value); }
        public bool Tank1Su { get => GetBit(5, 5); set => SetBit(5, 5, value); }
        public bool Tank1Dozaj { get => GetBit(5, 9); set => SetBit(5, 9, value); }
    }

    public class BosaltmaParams : BaseStepParams
    {
        public BosaltmaParams(short[] data) : base(data) { }

        // Word 10: Sağ-Sol Yön Süre (SN)
        public short SagSolSure { get => _data[10]; set => _data[10] = value; }
        // Word 15: Bekleme Zamanı (SN)
        public short BeklemeZamani { get => _data[15]; set => _data[15] = value; }
        // Word 12: Çalışma Devri
        public short CalismaDevri { get => _data[12]; set => _data[12] = value; }

        // Word 13: Kontrol Bitleri
        public bool TamburDur { get => GetBit(13, 0); set => SetBit(13, 0, value); }
        public bool Alarm { get => GetBit(13, 1); set => SetBit(13, 1, value); }
    }

    public class SikmaParams : BaseStepParams
    {
        public SikmaParams(short[] data) : base(data) { }

        // Word 8: Sıkma Devri
        public short SikmaDevri { get => _data[8]; set => _data[8] = value; }
        // Word 9: Sıkma Süre (DK)
        public short SikmaSure { get => _data[9]; set => _data[9] = value; }
    }
    public class KurutmaParams : BaseStepParams
    {
        public KurutmaParams(short[] data) : base(data) { }

        public short Temperature { get => _data[0]; set => _data[0] = value; }
        public short Humidity { get => _data[1]; set => _data[1] = value; }
        public short DurationMinutes { get => _data[2]; set => _data[2] = value; }
        public short Rpm { get => _data[3]; set => _data[3] = value; }
        public short CoolingTimeMinutes { get => _data[4]; set => _data[4] = value; }
        public short ControlWord { get => _data[5]; set => _data[5] = value; }

        public bool HumidityControlActive { get => GetBit(5, 0); set => SetBit(5, 0, value); }
        public bool TimeControlActive { get => GetBit(5, 1); set => SetBit(5, 1, value); }
        public bool Alarm { get => GetBit(5, 2); set => SetBit(5, 2, value); }
    }

    #endregion
}
