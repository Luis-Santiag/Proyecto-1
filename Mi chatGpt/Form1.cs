using System;
using System.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenAI_API; // Biblioteca de OpenAI
using OpenAI_API.Chat;
using Word = Microsoft.Office.Interop.Word;
using PowerPoint = Microsoft.Office.Interop.PowerPoint;

namespace Mi_chatGpt
{
    public partial class Form1 : Form
    {
        private string apiKey = "Clave "; // Reemplaza con tu clave de OpenAI
        private string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SqlServerConn"].ConnectionString;
        private string respuestaApi = string.Empty;
        private OpenAIAPI openAiApi;

        public Form1()
        {
            InitializeComponent();
            openAiApi = new OpenAIAPI(apiKey); // Inicializa la API de OpenAI
        }

        // Botón Investigar
        private async void btnInvestigar_Click(object sender, EventArgs e)
        {
            string prompt = txtPrompt.Text;
            if (string.IsNullOrEmpty(prompt))
            {
                MessageBox.Show("Ingrese un prompt para investigar.");
                return;
            }

            lblEstado.Text = "Consultando API de ChatGPT...";
            respuestaApi = await ConsultarChatGptApi(prompt);
            txtRespuesta.Text = respuestaApi;
            lblEstado.Text = "Consulta completada. Revise el resultado.";

            // Guardar en base de datos
            GuardarEnBaseDatos(prompt, respuestaApi);

            // Estimar tokens (aproximado)
            int tokensEstimados = (prompt.Length + respuestaApi.Length) / 4;
            lblEstado.Text += $"\nTokens estimados: {tokensEstimados}";
        }

        // Botón Aprobar
        private void btnAprobar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(respuestaApi))
            {
                MessageBox.Show("No hay resultados para generar archivos.");
                return;
            }

            string carpeta = GenerarArchivos(txtPrompt.Text, respuestaApi);
            lblEstado.Text = $"Archivos generados en: {carpeta}";
        }

        // Botón Editar
        private void btnEditar_Click(object sender, EventArgs e)
        {
            respuestaApi = txtRespuesta.Text; // Actualiza la respuesta con los cambios
            lblEstado.Text = "Respuesta editada. Presione Aprobar para generar archivos.";
        }

        // Consultar API de ChatGPT
        private async Task<string> ConsultarChatGptApi(string prompt)
        {
            try
            {
                var chatRequest = openAiApi.Chat.CreateConversation();
                chatRequest.AppendUserInput(prompt);
                chatRequest.RequestParameters.Model = "gpt-3.5-turbo"; // Usa GPT-3.5-turbo para costos bajos
                chatRequest.RequestParameters.MaxTokens = 500; // Límite de tokens en la respuesta
                chatRequest.RequestParameters.Temperature = 0.7; // Controla la creatividad (0 a 1)

                var response = await chatRequest.GetResponseFromChatbotAsync();
                return response;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al consultar la API de ChatGPT: " + ex.Message);
                return string.Empty;
            }
        }

        // Guardar en SQL Server
        private void GuardarEnBaseDatos(string prompt, string respuesta)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "INSERT INTO Investigaciones (Prompt, Respuesta, Fecha) VALUES (@Prompt, @Respuesta, @Fecha)";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Prompt", prompt);
                        cmd.Parameters.AddWithValue("@Respuesta", respuesta);
                        cmd.Parameters.AddWithValue("@Fecha", DateTime.Now);
                        cmd.ExecuteNonQuery();
                    }
                }
                MessageBox.Show("Datos guardados en la base de datos.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar en la base de datos: " + ex.Message);
            }
        }

        // Generar Word y PowerPoint
        private string GenerarArchivos(string prompt, string respuesta)
        {
            try
            {
                if (string.IsNullOrEmpty(prompt) || string.IsNullOrEmpty(respuesta))
                {
                    MessageBox.Show("El prompt o la respuesta están vacíos."); // Esto aún muestra un mensaje
                    return string.Empty;
                }

                

                string carpeta = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), $"Investigacion_{DateTime.Now:yyyyMMdd_HHmmss}");
                Directory.CreateDirectory(carpeta);

                string wordPath = Path.Combine(carpeta, "Investigacion.docx");
                
                GenerarDocumentoWord(wordPath, prompt, respuesta);

                string pptPath = Path.Combine(carpeta, "Investigacion.pptx");
               
                GenerarPresentacionPowerPoint(pptPath, prompt, respuesta);

                return carpeta;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al generar archivos: {ex.Message}\n{ex.StackTrace}"); // Esto aún muestra un mensaje
                return string.Empty;
            }
        }

        // Generar documento Word usando plantilla
        private void GenerarDocumentoWord(string path, string prompt, string respuesta)
        {
            Word.Application wordApp = null;
            Word.Document doc = null;

            try
            {
                wordApp = new Word.Application();
                wordApp.Visible = false; 
                wordApp.DisplayAlerts = Word.WdAlertLevel.wdAlertsNone; // Deshabilitar alertas

                MessageBox.Show("Creando un nuevo documento Word..."); // Solo para depuración, quítalo si es automático
                doc = wordApp.Documents.Add(); // Crear documento en blanco

                // Agregar contenido directamente
                MessageBox.Show("Agregando contenido al documento..."); // Solo para depuración
                Word.Paragraph para1 = doc.Content.Paragraphs.Add();
                para1.Range.Text = "Título de la Investigación:";
                para1.Range.Font.Bold = 1; // Negrita
                para1.Range.InsertParagraphAfter();

                Word.Paragraph para2 = doc.Content.Paragraphs.Add();
                para2.Range.Text = prompt ?? "";
                para2.Range.InsertParagraphAfter();

                Word.Paragraph para3 = doc.Content.Paragraphs.Add();
                para3.Range.Text = "Resultados:";
                para3.Range.Font.Bold = 1;
                para3.Range.InsertParagraphAfter();

                // Dividir la respuesta en partes de 5000 caracteres
                const int chunkSize = 5000;
                int startIndex = 0;
                while (startIndex < respuesta.Length)
                {
                    int length = Math.Min(chunkSize, respuesta.Length - startIndex);
                    string chunk = respuesta.Substring(startIndex, length);
                    Word.Paragraph para = doc.Content.Paragraphs.Add();
                    para.Range.Text = chunk;
                    para.Range.InsertParagraphAfter();
                    startIndex += length;
                }

                MessageBox.Show($"Guardando documento en: {path}"); // Solo para depuración
                doc.SaveAs2(path);
                doc.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al generar Word: {ex.Message}\n{ex.StackTrace}");
            }
            finally
            {
                if (wordApp != null)
                {
                    try
                    {
                        wordApp.DisplayAlerts = Word.WdAlertLevel.wdAlertsAll; // Restaurar alertas
                        wordApp.Quit(); // Cerrar Word
                    }
                    catch { }
                }
                if (doc != null)
                {
                    try { doc.Close(false); } catch { }
                }
            }
        }

        // Generar presentación PowerPoint
        private void GenerarPresentacionPowerPoint(string path, string prompt, string respuesta)
        {
            PowerPoint.Application pptApp = new PowerPoint.Application();
            PowerPoint.Presentation ppt = pptApp.Presentations.Add();

            try
            {
                // Diapositiva 1: Título
                var slide1 = ppt.Slides.Add(1, PowerPoint.PpSlideLayout.ppLayoutText);
                slide1.Shapes.Title.TextFrame.TextRange.Text = "Investigación: " + prompt;

                // Diapositiva 2: Contenido
                var slide2 = ppt.Slides.Add(2, PowerPoint.PpSlideLayout.ppLayoutText);
                slide2.Shapes.Title.TextFrame.TextRange.Text = "Resultados";
                slide2.Shapes[2].TextFrame.TextRange.Text = respuesta;

                ppt.SaveAs(path);
                ppt.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al generar PowerPoint: " + ex.Message);
            }
            finally
            {
                pptApp.Quit();
            }
        }
    }
}

