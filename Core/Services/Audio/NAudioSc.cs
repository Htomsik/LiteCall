using System.Collections.Concurrent;
using Core.Models.AppInfrastructure;
using Core.Models.Servers.Messages;
using Core.Services.Interfaces.Audio;
using Core.Stores.AppInfrastructure;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using ReactiveUI;

namespace Core.Services.Audio;

public class NAudioSc : IAudioSc, IDisposable
{
    private readonly WaveFormat _waveFormat = new(16000, 16, 1);
    private WaveOutEvent? _output;
    private WaveInEvent? _input;
    private readonly MixingSampleProvider _mixer;

    private readonly AppSettings _appSettings;
    
    private bool _inputRecording;
    
    /// Buffers for all voiced users
    private readonly ConcurrentDictionary<string, BufferedWaveProvider> _bufferUsers = new();
    
    /// Send input data to outer sources 
    public event Action<byte[]>? InputDataGenerated;

    public NAudioSc(AppSettingsStore appSettingsStore)
    {
        _appSettings = appSettingsStore.CurrentValue;
        
        // input settings
        InitializeInput();

        // Output settings
        _mixer = new MixingSampleProvider(WaveFormat.CreateIeeeFloatWaveFormat(16000, 1)) { ReadFully = true };
        InitializeOutput();

        this.WhenAnyValue(x => x._appSettings.OutputDeviceId)
            .Subscribe(x => InitializeOutput());
        
        this.WhenAnyValue(x => x._appSettings.CaptureDeviceId)
            .Subscribe(x => InitializeOutput());
    }
    
    private void InitializeInput()
    {
        if (_input != null)
        {
            _input.RecordingStopped -= OnRecordingStopped;
            _input.DataAvailable -= OnDataAvailable;
            _input.StopRecording();
            _input.Dispose();
        }

        try
        {
            // NAudio can throw error if device out of range
            _input = new WaveInEvent
            {
                BufferMilliseconds = 25, 
                WaveFormat = _waveFormat,
                DeviceNumber = _appSettings.CaptureDeviceId
            };
        }
        catch (Exception e)
        {
            _input = new WaveInEvent
            {
                BufferMilliseconds = 25, 
                WaveFormat = _waveFormat,
                DeviceNumber =  0
            };

            _appSettings.CaptureDeviceId = 0;
        }
        
        _input.DataAvailable += OnDataAvailable;
        _input.RecordingStopped += OnRecordingStopped;

        if (_inputRecording)
        {
            StartRecording();
        }
    }
    
    private void InitializeOutput()
    {
        if (_output != null)
        {
            _output.Stop();
            _output.Dispose();
        }

        try
        {
            // NAudio can throw error if device out of range
            _output = new WaveOutEvent { DeviceNumber = _appSettings.OutputDeviceId };
        }
        catch (Exception e)
        {
            _output = new WaveOutEvent { DeviceNumber = 0 };
            _output.Init(_mixer);
            
            _appSettings.OutputDeviceId = 0;
        }
        
        _output.Init(_mixer);
        _output.Play();
    }

    private void OnRecordingStopped(object? sender, StoppedEventArgs e)
    {
        _inputRecording = false;
    }

    /// Capture microphone sound 
    private void OnDataAvailable(object? sender, WaveInEventArgs e)
    {
        InputDataGenerated?.Invoke(e.Buffer);
    }
    
    public void StartRecording()
    {
        try
        {
            _input?.StartRecording();
            _inputRecording = true;
        }
        catch
        {
             /* ignored */
        }
    }

    public void StopRecording()
    {
        try { _input?.StopRecording(); } catch { /* ignored */ }
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

        if (_output?.PlaybackState == PlaybackState.Stopped) 
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
        if (_input != null)
        {
            StopRecording();
            _input.RecordingStopped -= OnRecordingStopped;
            _input.DataAvailable -= OnDataAvailable;
            _input.Dispose();
        }

        if (_output != null)
        {
            _output.Stop();
            _output.Dispose();
        }
        
        _bufferUsers.Clear();
    }
}