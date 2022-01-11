using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using Newtonsoft.Json.Serialization;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Runtime.Serialization;
using System.Data.SqlClient;

namespace LecturaJsonResponse
{
    class Program
    {
        public class Data
        {
            public string libro { get; set; }
            public string categoria { get; set; }
            public string descripcion { get; set; }
        }

        public class Root
        {
            public string status { get; set; }
            public string time_result { get; set; }
            public Data data { get; set; }
        }

        public class Book
        {
            public string isbn { get; set; }
            public string api_key { get; set; }
        }

        public class addBook
        {
            public string isbn { get; set; }
            public string api_key { get; set; }
            public string nombre { get; set; }
            public string descripcion { get; set; }
            public string categoria { get; set; }
        }

        public static string JsonConver { get; private set; }

        static void Main(string[] args)
        {
            SqlConnection conexionDB = new SqlConnection("Data Source = localhost; Initial Catalog = bibliotecum; Integrated Security = True");
            conexionDB.Open();

            int option = 0;

            while (option != 4)
            {
                printMenu();
                option = Int32.Parse(Console.ReadLine());

                if (option == 1)
                {
                    getBook(conexionDB);
                }
                if (option == 2)
                {
                    addbook();
                }
                if (option == 3)
                {
                    listbook("http://bibliotecum.herokuapp.com/api/listbook");
                }
            }

            //addData(rs.data.libro.ToString(), rs.data.descripcion.ToString(), rs.data.categoria.ToString(), conexionDB);

       
        }
        public static void printMenu()
        {
            Console.WriteLine("\n1-Get book");
            Console.WriteLine("\n2-Add book");
            Console.WriteLine("\n3-List book");
            Console.WriteLine("\n4-Salir");
        }

        public static void listbook(string url)
        {
            Console.WriteLine("\nIntroduce la api key: ");
            string apiKey = Console.ReadLine();

            string result = "";

            WebRequest request = WebRequest.Create(url);
            request.Method = "post";
            request.ContentType = "application/json; charset=UTF-8";

            using (var oSW = new StreamWriter(request.GetRequestStream()))
            {
                Book book = new Book() { api_key = apiKey, isbn = "0" };
                string Json = JsonConvert.SerializeObject(book, Formatting.Indented);

                oSW.Write(Json);
                oSW.Flush();
                oSW.Close();
            }

            WebResponse response = request.GetResponse();

            using (var oSR = new StreamReader(response.GetResponseStream()))
            {
                result = oSR.ReadToEnd().Trim();
            }

            using (var sr = new StringReader(result))
            {
                using (var sw = new StringWriter())
                {
                    var jr = new JsonTextReader(sr);
                    var jw = new JsonTextWriter(sw) { Formatting = Formatting.Indented };
                    jw.WriteToken(jr);

                    //TENGO QUE MOSTRAR LA VARIABLE SW//
                    
                    Console.WriteLine(sw.ToString());
                }
            }

            
        }

        public static void getBook(SqlConnection conexionDB)
        {
            string url = "http://bibliotecum.herokuapp.com/api/getbook";

            Console.WriteLine("\n\n\n\nIntroduce el isbn del libro: ");
            string isbnPost = Console.ReadLine();

            Console.WriteLine("Introduce la api key: ");
            string apiKeyPost = Console.ReadLine();

            string result = getPost(url, isbnPost, apiKeyPost);

            JObject jsonIdentado = JObject.Parse(result);

            var rs = JsonConvert.DeserializeObject<Root>(result);

            addData(rs.data.libro, rs.data.descripcion, rs.data.categoria, conexionDB);

            Console.WriteLine("\nNombre: " + rs.data.libro + "\nDescripcion: " + rs.data.descripcion + "\nCategoria: " + rs.data.categoria);
        }

        public static void addbook()
        {
            string url = "http://bibliotecum.herokuapp.com/api/addbook";

            Console.WriteLine("\n\n\n\nIntroduce la api key: ");
            string apiKeyPost = Console.ReadLine();

            Console.WriteLine("\nIntroduce un isbn: ");
            int isbn = Int32.Parse(Console.ReadLine());

            Console.WriteLine("\nIntroduce el nombre del libro: ");
            string nombre = Console.ReadLine();

            Console.WriteLine("\nIntroduce la descripcion del libro: ");
            string descriptcion = Console.ReadLine();

            Console.WriteLine("\nIntroduce la categoria del libro: ");
            string categoria = Console.ReadLine();

            string result = getPostAddBook(url, apiKeyPost, nombre, isbn, descriptcion, categoria);

            //JObject jsonIdentado = JObject.Parse(result);

            //var rs = JsonConvert.DeserializeObject<Root>(result);

        }

        public static string getPostAddBook(string url, string apiKey, string nombre, int isbn, string descripcion, string categoria)
        {
            string result = "";

            WebRequest request = WebRequest.Create(url);
            request.Method = "post";
            request.ContentType = "application/json; charset=UTF-8";

            using (var oSW = new StreamWriter(request.GetRequestStream()))
            {
                /*string json = "{" +
                    "\"api_key\" : \"pepe\"," +
                    "\"isbn\" : " + isbn + "," +
                    "\"nombre\" : " + nombre + "," +
                    "\"descripcion\" : " + descripcion + "," +
                    "\"categoria\" : " + categoria + "}";*/
                addBook Addbook = new addBook() { api_key = apiKey, isbn = isbn.ToString(), nombre = nombre, descripcion = descripcion, categoria = categoria };


                string Json = JsonConvert.SerializeObject(Addbook, Formatting.Indented);

                oSW.Write(Json);
                oSW.Flush();
                oSW.Close();
            }

            WebResponse response = request.GetResponse();

            using (var oSR = new StreamReader(response.GetResponseStream()))
            {
                result = oSR.ReadToEnd().Trim();
            }
            return result;
        }

        public static string getPost(string url, string isbnPost, string apiKeyPost)
        {
            Book book = new Book() { isbn = isbnPost, api_key = apiKeyPost };
            string result = "";

            WebRequest request = WebRequest.Create(url);
            request.Method = "post";
            request.ContentType = "application/json; charset=UTF-8";

            using (var oSW = new StreamWriter(request.GetRequestStream()))
            {
                //string json = "{\"isbn\" : \"1234\", \"api_key\" : \"pepe\"}";
                string json = JsonConvert.SerializeObject(book, Formatting.Indented);

                oSW.Write(json);
                oSW.Flush();
                oSW.Close();
            }

            WebResponse response = request.GetResponse();

            using (var oSR = new StreamReader(response.GetResponseStream()))
            {
                result = oSR.ReadToEnd().Trim();
            }

            return result;
        }

        public static void addData(string nombre, string descripcion, string categoria, SqlConnection conexionDB)
        {
            string query = "INSERT INTO libros (nombre, descripcion, categoria) VALUES ('" + nombre + "', '" + descripcion + "', '" + categoria + "')";

            SqlCommand comando = new SqlCommand(query, conexionDB);
            int r = comando.ExecuteNonQuery();
        }
    }
}
