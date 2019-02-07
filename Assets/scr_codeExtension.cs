using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Random = UnityEngine.Random;

namespace CodeExtension {
	public static class scr_codeExtension {
		/// <summary>
		/// Converts the given elements to a string array.
		/// </summary>
		/// <returns>The string array.</returns>
		/// <param name="toConv">To conv.</param>
		public static string[] ToStringArray<T>(this IEnumerable<T> toConv) {
			return toConv.OfType<object>().Select(x => x.ToString()).ToArray();
		}

		/// <summary>
		/// Converts the given elements to string separated by rows and columns.
		/// </summary>
		/// <returns>The array.</returns>
		/// <param name="toLog">To log.</param>
		/// <param name="logSep">Log sep.</param>
		/// <param name="rowLen">Row length.</param>
		/// <param name="colLen">Col length.</param>
		public static string JoinGrid<T>(this IEnumerable<T> toLog, string logSep, int rowLen, int colLen) {
			var fullLog = new string[rowLen];

			for (int i = 0; i < fullLog.Length; i++) {
				fullLog[i] = string.Format("[{0}] {1}\n", i, toLog.Slice(colLen * i, colLen).Join(logSep));
			}

			return fullLog.Join("");
		}

		/// <summary>
		/// Shuffles the specified elements.
		/// </summary>
		/// <param name="toShuff">To shuff.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public static T Shuffle<T>(this T toShuff) where T : IList {
			if (toShuff == null) {
				throw new ArgumentNullException("toShuff");
			}

			for (int j = toShuff.Count; j >= 1; j--) {
				var item = Random.Range(0, j);

				if (item < j - 1) {
					var temp = toShuff[item];
					toShuff[item] = toShuff[j - 1];
					toShuff[j - 1] = temp;
				}
			}

			return toShuff;
		}

		/// <summary>
		/// Determines wether the specified int is even.
		/// </summary>
		/// <returns><c>true</c> if is even the specified numCheck; otherwise, <c>false</c>.</returns>
		/// <param name="numCheck">Number check.</param>
		public static bool IsEven(this int numCheck) {
			return (numCheck % 2 == 0);
		}

		/// <summary>
		/// Returns the factorial of the specified int.
		/// </summary>
		/// <param name="toFact">To fact.</param>
		public static int Fact(this int toFact) {
			return (toFact > 1) ? toFact * Fact(toFact - 1) : 1;
		}

		/// <summary>
		/// Determines if the given int is within the given range
		/// </summary>
		/// <returns><c>true</c> if is in range the specified numCheck minVal maxVal; otherwise, <c>false</c>.</returns>
		/// <param name="numCheck">Number check.</param>
		/// <param name="minVal">Minimum value.</param>
		/// <param name="maxVal">Max value.</param>
		public static bool IsInRange(this int numCheck, int minVal, int maxVal) {
			return (numCheck >= minVal && numCheck <= maxVal);
		}

		/// <summary>
		/// Slices the specified element.
		/// </summary>
		/// <param name="toSlice">To slice.</param>
		/// <param name="startFrom">Start from.</param>
		/// <param name="itemCount">Item count.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public static IEnumerable<T> Slice<T>(this IEnumerable<T> toSlice, int startFrom, int itemCount) {
			return toSlice.Skip(startFrom).Take(itemCount);
		}

		/// <summary>
		/// Converts the given int to bool.
		/// </summary>
		/// <returns><c>true</c>, if bool was toed, <c>false</c> otherwise.</returns>
		/// <param name="toBool">To bool.</param>
		public static bool ToBool(this int toBool) {
			return (toBool >= 1) ? true : false;
		}

		/// <summary>
		/// Makes the current int array to bool array.
		/// </summary>
		/// <returns>The bool array.</returns>
		/// <param name="toBoolArr">To bool arr.</param>
		public static bool[] ToBoolArray(this int[] toBoolArr) {
			return toBoolArr.Select(x => x.ToBool()).ToArray();
		}

		/// <summary>
		/// Converts the given bool to int.
		/// </summary>
		/// <returns><c>true</c>, if int was toed, <c>false</c> otherwise.</returns>
		/// <param name="toInt">If set to <c>true</c> to int.</param>
		public static int ToInt(this bool toInt) {
			return (toInt) ? 1 : 0;
		}

		/// <summary>
		/// Makes the current bool array to int array.
		/// </summary>
		/// <returns>The int array.</returns>
		/// <param name="toIntArr">To int arr.</param>
		public static int[] ToIntArray(this bool[] toIntArr) {
			return toIntArr.Select(x => x.ToInt()).ToArray();
		}

		/// <summary>
		/// Converts the given 2D array to 1D array.
		/// </summary>
		/// <returns>The array1 d.</returns>
		/// <param name="toArrayOD">To array O.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public static T[] ToArray1D<T>(this T[,] toArrayOD) {
			var rowLen = toArrayOD.GetLength(0);
			var colLen = toArrayOD.GetLength(1);
			T[] setArrayOD = new T[rowLen * colLen];

			for (int i = 0; i < rowLen; i++) {
				for (int j = 0; j < colLen; j++) {
					setArrayOD[(colLen * i) + j] = toArrayOD[i, j];
				}
			}

			return setArrayOD;
		}

		/// <summary>
		/// Converts the given 1D array to 2D array.
		/// </summary>
		/// <returns>The array2 d.</returns>
		/// <param name="toArrayTD">To array T.</param>
		/// <param name="rowLen">Row length.</param>
		/// <param name="colLen">Col length.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public static T[,] ToArray2D<T>(this T[] toArrayTD, int rowLen, int colLen) {
			T[,] setArrayTD = new T[rowLen, colLen];

			for (int i = 0; i < (rowLen * colLen); i++) {
				if (i < toArrayTD.Length) {
					setArrayTD[i / colLen, i % colLen] = toArrayTD[i];
				} else {
					setArrayTD[i / colLen, i % colLen] = default(T);
				}
			}

			return setArrayTD;
		}

        /// <summary>
        /// Returns the given int to coord based on column's length.
        /// </summary>
        /// <param name="nowPos"></param>
        /// <param name="colLength"></param>
        /// <returns></returns>
        public static string ToCoord(this int nowPos, int colLength) {
            var getRow = nowPos / colLength;
            var getCol = nowPos % colLength;

            return string.Format("{0}{1}", (char)(getCol + 65), getRow + 1);
        }

        /// <summary>
        /// Returns the given array into an object array.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="toObjArr"></param>
        /// <returns></returns>
        public static object[] ToObjectArray<T>(this IEnumerable<T> toObjArr) {
            return toObjArr.Select(x => (object)x).ToArray();
        }
	}
}