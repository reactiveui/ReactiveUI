#region License
//   Copyright 2010 John Sheehan
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License. 
#endregion

#region Modifications
//
// Modified by Kamran Ayub
//
// Changes:
// - switched inline RegEx calls to static variables to improve performance
// - added ToPascalCase to the GetNameVariants() call
//
#endregion

#region Further Modifications
//
// Modified by Anton Afanasyev
//
// Changes:
// - removed dashed variants since field names cannot have dashes
// - moved underscored names to the start of the try enumeration
//
#endregion

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace ReactiveUI
{
	/// <summary>
	/// String extensions to helper with name variants.
	/// Based on RestSharp library (see notice at top)
	/// </summary>
	public static class StringExtensions
	{

		private static readonly Regex RxIsUpperCase = new Regex( @"^[A-Z]+$" );

		private static readonly Regex RxPascalInner = new Regex( @"([A-Z]+)([A-Z][a-z])" );

		private static readonly Regex RxPascalOuter = new Regex( @"([a-z\d])([A-Z])" );

		private static readonly Regex RxHyphenSpaces = new Regex( @"[-\s]" );

		/// <summary>
		/// Converts a string to pascal case
		/// </summary>
		/// <param name="lowercaseAndUnderscoredWord">String to convert</param>
		/// <returns>string</returns>
		public static string ToPascalCase( this string lowercaseAndUnderscoredWord, CultureInfo culture )
		{
			return ToPascalCase( lowercaseAndUnderscoredWord, true, culture );
		}

		/// <summary>
		/// Converts a string to pascal case with the option to remove underscores
		/// </summary>
		/// <param name="text">String to convert</param>
		/// <param name="removeUnderscores">Option to remove underscores</param>
		/// <returns></returns>
		public static string ToPascalCase( this string text, bool removeUnderscores, CultureInfo culture )
		{
			if ( String.IsNullOrEmpty( text ) )
				return text;

			text = text.Replace( "_", " " );
			string joinString = removeUnderscores ? String.Empty : "_";
			string[] words = text.Split( ' ' );
			if ( words.Length > 1 || words[0].IsUpperCase() )
			{
				for ( int i = 0; i < words.Length; i++ )
				{
					if ( words[i].Length > 0 )
					{
						string word = words[i];
						string restOfWord = word.Substring( 1 );

						if ( restOfWord.IsUpperCase() )
							restOfWord = restOfWord.ToLower( culture );

						char firstChar = char.ToUpper( word[0], culture );
						words[i] = String.Concat( firstChar, restOfWord );
					}
				}
				return String.Join( joinString, words );
			}
			return String.Concat( words[0].Substring( 0, 1 ).ToUpper( culture ), words[0].Substring( 1 ) );
		}

		/// <summary>
		/// Converts a string to camel case
		/// </summary>
		/// <param name="lowercaseAndUnderscoredWord">String to convert</param>
		/// <returns>String</returns>
		public static string ToCamelCase( this string lowercaseAndUnderscoredWord, CultureInfo culture )
		{
			return MakeInitialLowerCase( ToPascalCase( lowercaseAndUnderscoredWord, culture ) );
		}

		/// <summary>
		/// Convert the first letter of a string to lower case
		/// </summary>
		/// <param name="word">String to convert</param>
		/// <returns>string</returns>
		public static string MakeInitialLowerCase( this string word )
		{
			return String.Concat( word.Substring( 0, 1 ).ToLower(), word.Substring( 1 ) );
		}

		/// <summary>
		/// Checks to see if a string is all uppper case
		/// </summary>
		/// <param name="inputString">String to check</param>
		/// <returns>bool</returns>
		public static bool IsUpperCase( this string inputString )
		{
			return RxIsUpperCase.IsMatch( inputString );
		}

		/// <summary>
		/// Add underscores to a pascal-cased string
		/// </summary>
		/// <param name="pascalCasedWord">String to convert</param>
		/// <returns>string</returns>
		public static string AddUnderscores( this string pascalCasedWord )
		{
			return
				RxHyphenSpaces.Replace(
					RxPascalOuter.Replace( RxPascalInner.Replace( pascalCasedWord, "$1_$2" ),
						"$1_$2" ), "_" );
		}

		/// <summary>
		/// Add an undescore prefix to a pascasl-cased string
		/// </summary>
		/// <param name="pascalCasedWord"></param>
		/// <returns></returns>
		public static string AddUnderscorePrefix( this string pascalCasedWord )
		{
			return "_" + pascalCasedWord;
		}

		/// <summary>
		/// Return possible variants of a name for name matching.
		/// </summary>
		/// <param name="name">String to convert</param>
		/// <param name="culture">The culture to use for conversion</param>
		/// <param name="options">The variants to try and match against (less is faster)</param>
		/// <returns>IEnumerable&lt;string&gt;</returns>
		public static IEnumerable<string> GetNameVariants( this string name, CultureInfo culture, NameVariants options )
		{
			if ( String.IsNullOrEmpty( name ) )
				yield break;

			// try name with _UnderscorePrefix
			if ( (options & NameVariants.UnderscorePrefix) == NameVariants.UnderscorePrefix )
				yield return name.AddUnderscorePrefix();

			// try name with _camelCasedUnderscorePrefix
			if ( (options & NameVariants.UnderscorePrefixCamelCase) == NameVariants.UnderscorePrefixCamelCase )
				yield return name.ToCamelCase( culture ).AddUnderscorePrefix();

			// try PascalCased
			if ( (options & NameVariants.PascalCase) == NameVariants.PascalCase )
				yield return name.ToPascalCase( true, culture );

			// try camelCased
			if ( (options & NameVariants.CamelCase) == NameVariants.CamelCase )
				yield return name.ToCamelCase( culture );

			// try lowercasedname
			if ( (options & NameVariants.Lowercase) == NameVariants.Lowercase )
				yield return name.ToLower( culture );

			// try Name_With_Underscores
			if ( (options & NameVariants.WithUnderscores) == NameVariants.WithUnderscores )
				yield return name.AddUnderscores();

			// try lowercased_name_with_underscores
			if ( (options & NameVariants.WithUnderscoresLowercase) == NameVariants.WithUnderscoresLowercase )
				yield return name.AddUnderscores().ToLower( culture );
		}
	}

	[Flags]
	public enum NameVariants
	{
		/// <summary>
		/// _UnderscorePrefix
		/// </summary>
		UnderscorePrefix,

		/// <summary>
		/// _underscorePrefix
		/// </summary>
		UnderscorePrefixCamelCase,

		/// <summary>
		/// PascalCased word
		/// </summary>
		PascalCase,

		/// <summary>
		/// camelCased word
		/// </summary>
		CamelCase,

		/// <summary>
		/// lowercase
		/// </summary>
		Lowercase,

		/// <summary>
		/// With_Underscores
		/// </summary>
		WithUnderscores,

		/// <summary>
		/// with_underscores
		/// </summary>
		WithUnderscoresLowercase,

		/// <summary>
		/// Considers all variants
		/// </summary>
		All = PascalCase | CamelCase | Lowercase | WithUnderscores | WithUnderscoresLowercase | UnderscorePrefix | UnderscorePrefixCamelCase
	}
}
