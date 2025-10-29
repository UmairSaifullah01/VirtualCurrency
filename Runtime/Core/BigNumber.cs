using System;
using System.Globalization;

namespace THEBADDEST.VirtualCurrencySystem
{
	[Serializable]
	public struct BigNumber : IEquatable<BigNumber>, IComparable<BigNumber>
	{
		// value = mantissa * (1000^exponent), mantissa in (-1000,-1] U [1,1000) or 0
		public double mantissa;
		public long exponent; // thousands exponent

		public BigNumber(double mantissa, long exponent)
		{
			this.mantissa = mantissa;
			this.exponent = exponent;
			Normalize();
		}

		public static BigNumber FromDouble(double value)
		{
			if (double.IsNaN(value) || double.IsInfinity(value))
				throw new ArgumentException("value must be a finite number");

			if (value == 0.0) return Zero;

			int sign = Math.Sign(value);
			double abs = Math.Abs(value);

			long exp = (long)Math.Floor(Math.Log10(abs) / 3.0);
			double scaled = abs / Math.Pow(1000.0, exp);

			if (scaled >= 1000.0) { scaled /= 1000.0; exp++; }
			if (scaled < 1.0) { scaled *= 1000.0; exp--; }

			return new BigNumber(sign * scaled, exp);
		}

		private static readonly double[] POW_1000_NEG =
		{
			1.0,
			1.0 / 1_000.0,
			1.0 / 1_000_000.0,
			1.0 / 1_000_000_000.0,
			1.0 / 1_000_000_000_000.0
		};

		private static double Pow1000Neg(long diff)
		{
			if (diff >= 0 && diff < POW_1000_NEG.Length) return POW_1000_NEG[diff];
			return Math.Pow(1000.0, -diff);
		}

		private void Normalize()
		{
			if (mantissa == 0.0 || double.IsNaN(mantissa) || double.IsInfinity(mantissa))
			{
				mantissa = 0.0;
				exponent = 0;
				return;
			}

			double abs = Math.Abs(mantissa);
			while (abs >= 1000.0)
			{
				mantissa /= 1000.0;
				exponent++;
				abs = Math.Abs(mantissa);
			}
			while (abs > 0.0 && abs < 1.0)
			{
				mantissa *= 1000.0;
				exponent--;
				abs = Math.Abs(mantissa);
			}
		}

		public static readonly BigNumber Zero = new BigNumber(0.0, 0);
		public static readonly BigNumber One = new BigNumber(1.0, 0);
		public static readonly BigNumber Thousand = new BigNumber(1.0, 1);

		public static BigNumber Add(in BigNumber a, in BigNumber b)
		{
			if (a.mantissa == 0.0) return b;
			if (b.mantissa == 0.0) return a;

			BigNumber big = a, small = b;
			if (big.exponent < small.exponent) { big = b; small = a; }

			long diff = big.exponent - small.exponent;
			if (diff >= 20) return big; // small is negligible

			double smallScaled = small.mantissa * Pow1000Neg(diff);
			var res = new BigNumber(big.mantissa + smallScaled, big.exponent);
			res.Normalize();
			return res;
		}

		public static BigNumber Subtract(in BigNumber a, in BigNumber b)
		{
			if (b.mantissa == 0.0) return a;
			if (a.mantissa == 0.0) return new BigNumber(-b.mantissa, b.exponent);

			BigNumber big = a, small = b;
			bool swapped = false;
			if (big.exponent < small.exponent) { big = b; small = a; swapped = true; }

			long diff = big.exponent - small.exponent;
			if (diff >= 20) return swapped ? new BigNumber(-big.mantissa, big.exponent) : big;

			double smallScaled = small.mantissa * Pow1000Neg(diff);
			var res = new BigNumber(big.mantissa - smallScaled, big.exponent);
			res.Normalize();
			return swapped ? new BigNumber(-res.mantissa, res.exponent) : res;
		}

		public static BigNumber Multiply(in BigNumber a, double scalar)
		{
			if (scalar == 0.0 || a.mantissa == 0.0) return Zero;
			var res = new BigNumber(a.mantissa * scalar, a.exponent);
			res.Normalize();
			return res;
		}

		public static BigNumber Multiply(in BigNumber a, in BigNumber b)
		{
			if (a.mantissa == 0.0 || b.mantissa == 0.0) return Zero;
			var res = new BigNumber(a.mantissa * b.mantissa, a.exponent + b.exponent);
			res.Normalize();
			return res;
		}

		public static BigNumber Divide(in BigNumber a, double scalar)
		{
			if (scalar == 0.0) throw new DivideByZeroException();
			var res = new BigNumber(a.mantissa / scalar, a.exponent);
			res.Normalize();
			return res;
		}

		public static BigNumber Divide(in BigNumber a, in BigNumber b)
		{
			if (b.mantissa == 0.0) throw new DivideByZeroException();
			var res = new BigNumber(a.mantissa / b.mantissa, a.exponent - b.exponent);
			res.Normalize();
			return res;
		}

		public static BigNumber operator +(BigNumber a, BigNumber b) => Add(a, b);
		public static BigNumber operator -(BigNumber a, BigNumber b) => Subtract(a, b);
		public static BigNumber operator *(BigNumber a, double s) => Multiply(a, s);
		public static BigNumber operator *(double s, BigNumber a) => Multiply(a, s);
		public static BigNumber operator /(BigNumber a, double s) => Divide(a, s);
		public static BigNumber operator *(BigNumber a, BigNumber b) => Multiply(a, b);
		public static BigNumber operator /(BigNumber a, BigNumber b) => Divide(a, b);

		public int CompareTo(BigNumber other)
		{
			if (exponent != other.exponent) return exponent.CompareTo(other.exponent);
			return mantissa.CompareTo(other.mantissa);
		}

		public static bool operator <(BigNumber a, BigNumber b) => a.CompareTo(b) < 0;
		public static bool operator >(BigNumber a, BigNumber b) => a.CompareTo(b) > 0;
		public static bool operator <=(BigNumber a, BigNumber b) => a.CompareTo(b) <= 0;
		public static bool operator >=(BigNumber a, BigNumber b) => a.CompareTo(b) >= 0;
		public static bool operator ==(BigNumber a, BigNumber b) => a.Equals(b);
		public static bool operator !=(BigNumber a, BigNumber b) => !a.Equals(b);

		public bool Equals(BigNumber other)
		{
			const double Epsilon = 1e-10;
			return Math.Abs(mantissa - other.mantissa) < Epsilon && exponent == other.exponent;
		}

		public override bool Equals(object obj) => obj is BigNumber other && Equals(other);
		public override int GetHashCode() => HashCode.Combine(mantissa, exponent);

		private static readonly string[] COMMON_SUFFIXES = { "", "K", "M", "B", "T" };

		public string ToStringFormatted(int decimals = 2)
		{
			if (mantissa == 0.0) return "0";

			if (exponent < 1)
			{
				double value = mantissa * Math.Pow(1000.0, exponent);
				return value.ToString("N" + decimals, CultureInfo.InvariantCulture);
			}

			string suffix = GetSuffixForExponent(exponent);
			string fmt = "F" + decimals;
			return mantissa.ToString(fmt, CultureInfo.InvariantCulture) + suffix;
		}

		private static string GetSuffixForExponent(long exp)
		{
			if (exp < 0) return string.Empty;
			if (exp < COMMON_SUFFIXES.Length) return COMMON_SUFFIXES[exp];

			long alphaIndex = exp - COMMON_SUFFIXES.Length; // 0 => aa
			string s = string.Empty;
			long index = alphaIndex;
			do
			{
				char ch = (char)('a' + (index % 26));
				s = ch + s;
				index /= 26;
			}
			while (s.Length < 2 || index > 0);

			return s;
		}

		public double ToDouble() => mantissa * Math.Pow(1000.0, exponent);

		public override string ToString() => ToStringFormatted(2);

		public string ToCompactString()
		{
			return mantissa.ToString("R", CultureInfo.InvariantCulture) + "|" + exponent.ToString(CultureInfo.InvariantCulture);
		}

		public static BigNumber FromCompactString(string s)
		{
			if (string.IsNullOrEmpty(s)) return Zero;
			var parts = s.Split('|');
			if (parts.Length != 2) return Zero;
			if (!double.TryParse(parts[0], NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var m)) return Zero;
			if (!long.TryParse(parts[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out var e)) return Zero;
			return new BigNumber(m, e);
		}
	}
}


