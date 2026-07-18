from pathlib import Path
import wave

import numpy as np


OUTPUT_DIRECTORY = Path(__file__).resolve().parent
QUALITY = 50


def quantize_to_int16(samples: np.ndarray) -> np.ndarray:
    scaled = np.trunc(samples * 32768.0)
    clipped = np.clip(scaled, -32768, 32767)
    return clipped.astype("<i2")


def write_wave(path: Path, samples: np.ndarray, sample_rate: int) -> None:
    encoded = quantize_to_int16(samples)
    with wave.open(str(path), "wb") as destination:
        destination.setnchannels(1)
        destination.setsampwidth(2)
        destination.setframerate(sample_rate)
        destination.writeframes(encoded.tobytes())


def read_wave(path: Path) -> np.ndarray:
    with wave.open(str(path), "rb") as source:
        if source.getnchannels() != 1 or source.getsampwidth() != 2:
            raise ValueError("The source WAV must be monaural 16-bit PCM.")
        encoded = np.frombuffer(source.readframes(source.getnframes()), dtype="<i2")
    return encoded.astype(np.float64) / 32768.0


def create_noise(sample_count: int, seed: int) -> np.ndarray:
    random = np.random.default_rng(seed)
    return random.uniform(-0.5, 0.5, sample_count)


def resample_lanczos(source: np.ndarray, p: int, q: int, a: int) -> np.ndarray:
    destination_length = int(np.ceil(source.size * p / q))
    destination = np.empty(destination_length, dtype=np.float64)

    sinc_factor = 1.0 if p > q else q / p
    gain = 1.0 if p > q else p / q

    for destination_index in range(destination_length):
        position = destination_index * q / p
        left = max(0, int(np.floor(position - a * sinc_factor)) + 1)
        right = min(source.size, int(np.ceil(position + a * sinc_factor)))

        source_indices = np.arange(left, right)
        distance = source_indices - position
        kernel = np.sinc(distance / sinc_factor) * np.sinc(distance / (sinc_factor * a))
        destination[destination_index] = gain * np.sum(source[left:right] * kernel)

    return destination


noise16k = create_noise(16000, 42)
noise48k = create_noise(48000, 57)

write_wave(OUTPUT_DIRECTORY / "noise16k.wav", noise16k, 16000)
write_wave(OUTPUT_DIRECTORY / "noise48k.wav", noise48k, 48000)

# Read back the actual 16-bit PCM inputs so Python and WaveFile.ReadMono resample identical data.
noise16k = read_wave(OUTPUT_DIRECTORY / "noise16k.wav")
noise48k = read_wave(OUTPUT_DIRECTORY / "noise48k.wav")
noise16k_to_48k = resample_lanczos(noise16k, 3, 1, QUALITY)
noise48k_to_16k = resample_lanczos(noise48k, 1, 3, QUALITY)

write_wave(OUTPUT_DIRECTORY / "noise16kto48k.wav", noise16k_to_48k, 48000)
write_wave(OUTPUT_DIRECTORY / "noise48kto16k.wav", noise48k_to_16k, 16000)
