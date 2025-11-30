using System;
using System.Windows.Controls;
using System.Windows;

namespace KeyceWordEditor.Extensions
{
    public class VoiceModule
    {
        private bool isListening = false;
        private RichTextBox? currentEditor; // ✅ Correct - nullable

        public VoiceModule()
        {
            // Pas d'initialisation de System.Speech
        }

        public void ToggleDictation(RichTextBox editor)
        {
            currentEditor = editor;

            if (!isListening)
            {
                isListening = true;
                MessageBox.Show("Dictée vocale activée (fonctionnalité simulée)");
            }
            else
            {
                isListening = false;
                MessageBox.Show("Dictée vocale désactivée");
            }
        }
    }
}