using Microsoft.Xna.Framework.Content.Pipeline;

namespace OBJContentPipelineExtension
{
	[ContentProcessor(DisplayName = "OBJ Model Processor")]
	public class OBJProcessor : ContentProcessor<OBJFile, OBJFile>
	{
		public override OBJFile Process(OBJFile input, ContentProcessorContext context)
		{
			return input;
		}
	}
}
