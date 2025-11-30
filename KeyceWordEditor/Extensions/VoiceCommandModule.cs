using System;
using System.Windows;

namespace KeyceWordEditor.Extensions
{
    public class VoiceCommandModule
    {
        private readonly Action<string> commandAction;

        public VoiceCommandModule(Action<string> onCommandReceived)
        {
            commandAction = onCommandReceived;
        }

        public void StartListening()
        {
            try
            {
                MessageBox.Show("Commandes vocales activées (fonctionnalité simulée)",
                    "Commandes vocales", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur écoute: {ex.Message}");
            }
        }

        public void StopListening()
        {
        }
    }
}