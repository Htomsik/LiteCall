using System.Collections.Concurrent;
using Core.Models.Servers.Messages;
using Core.Services.Interfaces.AppInfrastructure;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace Core.Services.Audio;

public class NAudioService : IAudioSc, IDisposable
{
    private readonly WaveFormat _waveFormat = new(16000, 16, 1);
    
    private readonly WaveOutEvent _output;
    private readonly WaveInEvent _input;
    private readonly MixingSampleProvider _mixer;
    
    /// Buffers for all voiced users
    private readonly ConcurrentDictionary<string, BufferedWaveProvider> _bufferUsers = new();
    
    /// Send input data to outer sources 
    public event Action<byte[]>? InputDataGenerated;

    public NAudioService()
    {
        // input settings
        _input = new WaveInEvent { BufferMilliseconds = 25, WaveFormat = _waveFormat };
        _input.DataAvailable += OnDataAvailable;

        // Output settings
        _mixer = new MixingSampleProvider(WaveFormat.CreateIeeeFloatWaveFormat(16000, 1)) { ReadFully = true };
        _output = new WaveOutEvent { DeviceNumber = 0 };
        _output.Init(_mixer);
        _output.Play();
    }
    
    /// Capture microphone sound 
    private void OnDataAvailable(object? sender, WaveInEventArgs e)
    {
        InputDataGenerated?.Invoke(e.Buffer);
    }

    public void StartRecording()
    {
        try { _input.StartRecording(); } catch { /* ignored */ }
    }

    public void StopRecording()
    {
        try { _input.StopRecording(); } catch { /* ignored */ }
    }

    /// Playback input message from outer users
    public void PlayMessage(AudioMessage audioMessage)
    {
        if (string.IsNullOrEmpty(audioMessage.UserName) || audioMessage.Audio == null) 
            return;

        var bufferUser = _bufferUsers.GetOrAdd(audioMessage.UserName, _ =>
        {
            var newBuffer = new BufferedWaveProvider(_waveFormat)
            {
                DiscardOnBufferOverflow = true,
                BufferDuration = TimeSpan.FromSeconds(1),
                ReadFully = true
            };
            _mixer.AddMixerInput(newBuffer);
            return newBuffer;
        });
        bufferUser.AddSamples(audioMessage.Audio, 0, audioMessage.Audio.Length);

        if (_output.PlaybackState == PlaybackState.Stopped) 
            _output.Play();
    }
    
    /// Clear all audio buffers and mixer inputs 
    public void ClearAudio()
    {
        StopRecording();
        _mixer.RemoveAllMixerInputs();
        _bufferUsers.Clear();
    }
    
    
    public void Dispose()
    {
        _input.DataAvailable -= OnDataAvailable;
        
        StopRecording();
        
        _input.Dispose();
        _output.Stop();
        _output.Dispose();
        _bufferUsers.Clear();
    }
}