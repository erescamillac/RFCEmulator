using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace RFCEmulator
{
    public class InputReader {
        public InputReader() {
        }

        public string ReadString(string inputMsg, byte minLength) {
            string strFromKeyboard;
            bool error = false;
            do {
                if (error) {
                    Console.Error.WriteLine($"La cadena solicitada debe tener una longitud mínima de {minLength} caractéres SIN contar espacios en blanco al inicio o final de la misma.");
                    Console.WriteLine();
                }
                
                Console.Write(inputMsg);
                strFromKeyboard = Console.ReadLine();
                strFromKeyboard = strFromKeyboard != null ? strFromKeyboard.Trim() : "";
                error = strFromKeyboard.Length >= minLength ? false : true;
            } while (error);
            return strFromKeyboard;
        }

        public DateTime ReadBirthday() {
            DateTime birthday = DateTime.MinValue;
            CultureInfo mexicoCultureInfo = new CultureInfo("es-MX", false);
            string dateAsString, dateFormat = "dd/MM/yyyy";
            bool successParsing = false;
            do {
                Console.WriteLine();
                Console.Write($"Ingrese su fecha de Nacimiento en formato [{dateFormat}] ej. [03/12/2004]: ");
                dateAsString = Console.ReadLine();
                //
                successParsing = DateTime.TryParseExact(dateAsString, dateFormat, mexicoCultureInfo, DateTimeStyles.None, out birthday);
                if (!successParsing) {
                    Console.Error.WriteLine($"ERROR: La fecha ingresada {dateAsString} ES INVÁLIDA o NO se encuentra en el FORMATO ESPECIFICADO ({dateFormat}).");
                }
            } while (!successParsing);
            return birthday;
        }
    }//--fin: class InputReader

    public class PersonalData {

        private string _name;
        private string _firstLastName;
        private string _secondLastName;
        private DateTime _birthday;
        public PersonalData() {
            _name = "";
            _firstLastName = "";
            _secondLastName = "";
            _birthday = DateTime.Now;
        }

        public PersonalData(string name, string firstLastName, string secondLastName, DateTime birthday) {
            this.Name = name;
            this.FirstLastName = firstLastName;
            this.SecondLastName = secondLastName;
            this.Birthday = birthday;
        }

        public string Name {
            get {
                return _name;
            }
            set {
                _name = value;
            }
        }

        public string FirstLastName {
            get {
                return _firstLastName;
            }
            set {
                _firstLastName = value;
            }
        }

        public string SecondLastName {
            get {
                return _secondLastName;
            }
            set {
                _secondLastName = value;
            }
        }

        public DateTime Birthday {
            get {
                return _birthday;
            }
            set {
                _birthday = value;
            }
        }
    } //--fin: PersonalData
    public class RFCExtractor {

        private CharUtils cUtils;
        private PersonalData _personalData;
        private RandomGenerator _rGenerator;

        public RFCExtractor(PersonalData personalData) {
            cUtils = new CharUtils();
            _rGenerator = new RandomGenerator();
            _personalData = personalData;
        }
        private string ExtractFisrt2CharsFromApPat(string apPaterno) {
            StringBuilder sb = new StringBuilder();
            string trimmedApPat = null;
            bool vowelFound = false;
            if (apPaterno != null) {
                trimmedApPat = apPaterno.ToUpper().Trim();
            }

            if (trimmedApPat != null && trimmedApPat.Length > 1) {
                // Agregar 1ra letra del apellido paterno
                sb.Append(trimmedApPat.ElementAt(0));
                foreach (char c in trimmedApPat.Substring(1,trimmedApPat.Length - 1)) {
                    if (cUtils.IsVowel(c)) {
                        // Agregar primera vocal del apellido paterno
                        sb.Append(c);
                        vowelFound = true;
                        break;   
                    }
                }
                if (!vowelFound) {
                    sb.Append(trimmedApPat.ElementAt(1));
                }
            }
            return sb.ToString();
        }

        private char ExtractFirstChar(string str) {
            char c = Char.MinValue;
            string trimmedStr = null;
            if (str != null) {
                trimmedStr = str.Trim();
            }
            if (trimmedStr != null && trimmedStr.Length > 0) {
                c = trimmedStr.ElementAt(0);
            }
            return c;
        }

        private string RFCFormattedBirthday(DateTime birthday) {
            string formatStr = "yyMMdd", formattedDatetime = "";
            if (birthday != null) {
                formattedDatetime = birthday.ToString(formatStr);
            }
            return formattedDatetime;
        }

        public string GenerateRFC() {
            StringBuilder sb = new StringBuilder();
            string name;
            string[] multipleNames;
            //char[] separators = {' '};
            if (_personalData != null) {
                // en caso de tener más de un nombre SÓLO TOMAR EN CUENTA EL PRIMERO
                // .Split() por omisión utiliza los caracteres "en blanco" (whitespaces) como separador
                multipleNames = _personalData.Name.Split();

                if (multipleNames.Length > 1)
                {
                    name = multipleNames[0];
                }
                else {
                    name = _personalData.Name;
                }
                // Console.WriteLine($"name: [{name}]");
                // 1. Primera letra del apellido paterno + primera vocal
                sb.Append(ExtractFisrt2CharsFromApPat(_personalData.FirstLastName));
                // 2. Inicial del apellido materno.
                sb.Append(ExtractFirstChar(_personalData.SecondLastName));
                // 3. Inicial del nombre
                sb.Append(ExtractFirstChar(name));
                // 4. Fecha de nac. formateada RFC
                sb.Append(RFCFormattedBirthday(_personalData.Birthday));
                // 5. Finalizar con Homoclave
                sb.Append(_rGenerator.RandomHomoclave());
            }
            return sb.ToString();
        }
    }//--fin: class RFCExtractor

    public class CharUtils {
        public CharUtils() {
        }

        public bool IsVowel(char c) {
            bool isVowel = false;
            char character = Char.ToLower(c);
            if (character == 'a' || character == 'e' || character == 'i' || character == 'o' || character == 'u') {
                isVowel = true;
            }
            return isVowel;
        }
    }//--fin:class CharUtils
    public class RandomGenerator {

        private static Random random = new Random();
        private enum CharType {
            Character,
            Digit
        }
        public RandomGenerator() {
        }

        public string RandomHomoclave() {
            StringBuilder sb = new StringBuilder();
            /*
            for (byte i = 0; i < 3; i++) {
                if (GenerateCharOrDigit() == CharType.Character) {
                    sb.Append(RandomUpperCaseChar());
                } else {
                    sb.Append(RandomDigit());
                }
            } 
            */
            sb.Append(RandomDigit());
            sb.Append(RandomUpperCaseChar());
            sb.Append(RandomDigit());
            return sb.ToString();
        }

        private char RandomUpperCaseChar() {
            byte minVal = 65, maxVal = 90;
            int codePoint;
            codePoint = random.Next(minVal, maxVal + 1);
            return Convert.ToChar(codePoint);
        }

        private int RandomDigit() {
            byte minVal = 0, maxVal = 9;
            return random.Next(minVal, maxVal + 1);
        }

        private CharType GenerateCharOrDigit() {
            return random.NextDouble() > 0.5 ? CharType.Character : CharType.Digit ;
        }

    } //--fin: class RandomGenerator
    class Program
    {
        static void Main(string[] args)
        {
            InputReader iReader = new InputReader();
            PersonalData personalData;
            RFCExtractor rFCExtractor;
            string nombres, apPaterno, apMaterno;
            DateTime birthday;
            char continueP = 'n';
            do {
                Console.Clear();
                Console.WriteLine("##--Random Generator Test--##");

                nombres = iReader.ReadString("Nombre(s): ", 2);
                apPaterno = iReader.ReadString("Apellido paterno: ", 2);
                apMaterno = iReader.ReadString("Apellido materno: ", 2);

                //-- InputReader test
                birthday = iReader.ReadBirthday();

                personalData = new PersonalData(nombres, apPaterno, apMaterno, birthday);
                rFCExtractor = new RFCExtractor(personalData);

                Console.WriteLine($"RFC: {rFCExtractor.GenerateRFC()}");
                Console.WriteLine("¿Desea generar otro RFC? [y/n]: ");
                continueP = Console.ReadKey().KeyChar;
            } while (Char.ToLower(continueP).Equals('y'));
           
        }
    }
}
