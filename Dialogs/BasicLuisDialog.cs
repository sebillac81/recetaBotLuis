using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;

using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using LuisBot.Models;
using LuisBot;
using Microsoft.Bot.Connector;

namespace Microsoft.Bot.Sample.LuisBot
{
    // For more information about this template visit http://aka.ms/azurebots-csharp-luis
    [Serializable]
    public class BasicLuisDialog : LuisDialog<object>
    {
        private const string EntityRecetaName = "Receta";


        public BasicLuisDialog() : base(new LuisService(new LuisModelAttribute(
            ConfigurationManager.AppSettings["LuisAppId"], 
            ConfigurationManager.AppSettings["LuisAPIKey"], 
            domain: ConfigurationManager.AppSettings["LuisAPIHostName"])))
        {
        }

        [LuisIntent("None")]
        public async Task NoneIntent(IDialogContext context, LuisResult result)
        {
            await this.ShowLuisResult(context, result);
        }
        
        [LuisIntent("Saludo")]
        public async Task SaludoIntent(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Hola! mucho gusto. Soy RecetasBot y estoy para ayudarte a encontrar ricas recetas de acuerdo a tus gustos. Dime 'busco recetas light', 'quisiera alguna receta con pollo' or 'me gustaría cocinar comida mexicana'. E intentaré buscarte las mas ricas recetas...");

            context.Wait(this.MessageReceived);
        }

        //[LuisIntent("BusquedaReceta")]
        //public async Task BusquedaRecetaIntent(IDialogContext context, LuisResult result)
        //{
        //    //Cancel
        //    EntityRecommendation hotelEntityRecommendation;

        //    await this.ShowLuisResult(context, result);
        //}

        [LuisIntent("BusquedaReceta")]
        public async Task BusquedaRecetaIntent(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            var message = await activity;
            HeroCard attachment = null;
            await context.PostAsync($"Bienvenido al buscador de recetas! Analizando el siguiente mensaje: '{message.Text}'...");
            IList<Receta> lista = null;

            EntityRecommendation IngredienteEntityRecommendation;

            if (result.TryFindEntity("ingrediente", out IngredienteEntityRecommendation))
            {
                var resultMessage = context.MakeMessage();
                List<int> ids = new List<int>();
                int j = 0;
                resultMessage.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                resultMessage.Attachments = new List<Attachment>();


                switch (IngredienteEntityRecommendation.Entity?.ToString())
                {
                    case "dulce":
                        lista = await dbRepository.GetRecetasAsync("recetasDulces");
                        break;
                    case "light":
                        lista = await dbRepository.GetRecetasAsync("recetasLight");
                        break;
                    case "verduras":
                        lista = await dbRepository.GetRecetasAsync("recetasVerduras");
                        break;
                    case "carne":
                        lista = await dbRepository.GetRecetasAsync("recetasCarne");
                        break;
                    case "pollo":
                        lista = await dbRepository.GetRecetasAsync("recetasPollo");
                        break;
                    case "pescado":
                        lista = await dbRepository.GetRecetasAsync("recetasPescados");
                        break;
                    case "agridulce":
                        lista = await dbRepository.GetRecetasAsync("recetasAgridulces");
                        break;
                    case "mexicana":
                        lista = await dbRepository.GetRecetasAsync("recetasMexicanas");
                        break;
                    case "arabe":
                        lista = await dbRepository.GetRecetasAsync("recetasArabes");
                        break;
                    case "peruana":
                        lista = await dbRepository.GetRecetasAsync("recetasPeruanas");
                        break;
                    default:
                        break;
                }

                //muestro solo 3 elementos de la lista
                for (int i = 0; i < 3; i++)
                {
                    do
                    {
                        j = new Random().Next(lista.Count);
                    }
                    while (ids.Contains(j));
                    ids.Add(j);
                    
                    
                    attachment = new HeroCard()
                    {
                        Title = lista[j].Titulo,
                        Text = lista[j].Descripcion,
                        Images = new List<CardImage>() {
                            new CardImage(lista[j].Imagen)
                        },
                        Buttons = new List<CardAction>() {
                                new CardAction(ActionTypes.OpenUrl, title: "Ver receta", value: lista[j].Link)
                        }
                    };
                    resultMessage.Attachments.Add(attachment.ToAttachment());
                }

                await context.PostAsync(resultMessage);
            }

            context.Wait(this.MessageReceived);
        }

        private async Task ShowLuisResult(IDialogContext context, LuisResult result) 
        {
            await context.PostAsync($"La intención es:  {result.Intents[0].Intent}. Dijiste: {result.Query}");
            context.Wait(MessageReceived);
        }


        //private async Task<IEnumerable<Receta>> GetHotelsAsync(ent)
        //{
        //    var hotels = new List<Hotel>();

        //    // Filling the hotels results manually just for demo purposes
        //    for (int i = 1; i <= 5; i++)
        //    {
        //        var random = new Random(i);
        //        Hotel hotel = new Hotel()
        //        {
        //            Name = $"{searchQuery.Destination ?? searchQuery.AirportCode} Hotel {i}",
        //            Location = searchQuery.Destination ?? searchQuery.AirportCode,
        //            Rating = random.Next(1, 5),
        //            NumberOfReviews = random.Next(0, 5000),
        //            PriceStarting = random.Next(80, 450),
        //            Image = $"https://placeholdit.imgix.net/~text?txtsize=35&txt=Hotel+{i}&w=500&h=260"
        //        };

        //        hotels.Add(hotel);
        //    }

        //    hotels.Sort((h1, h2) => h1.PriceStarting.CompareTo(h2.PriceStarting));

        //    return hotels;
        //}
    }
}