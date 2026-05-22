using System;
using GriauleFingerprintLibrary;

public class Template
{
	public Template()
	{
        private void ExtractTemplate()
        {
            if (rawImage != null)
            {
                try
                {
                    fingerPrint.Extract(rawImage, ref _template);
                }
                catch
                {
                    LogMessage("Error al extraer la plantilla de la huella.");
                }
            }
        }
    }
}
