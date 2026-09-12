using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace BattleForDune.Core.Events
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public readonly struct GameEvent
    {
        [StructLayout(LayoutKind.Sequential, Size = 32)]
        private struct InlineBuffer { }

        public readonly GameEventType Type;

        private readonly InlineBuffer _payloadBuffer;

        private GameEvent(GameEventType type) => Type = type;

        public static GameEvent Create<T>(in T payload) where T : struct, IGameEvent
        {
            var command = new GameEvent(payload.Type);

            Span<byte> destination = MemoryMarshal.CreateSpan(
                ref Unsafe.As<InlineBuffer, byte>(ref Unsafe.AsRef(in command._payloadBuffer)),
                Unsafe.SizeOf<InlineBuffer>()
            );

            MemoryMarshal.Write(destination, in Unsafe.AsRef(in payload));
            return command;
        }

        public T ReadPayload<T>() where T : struct, IGameEvent
        {
            ReadOnlySpan<byte> source = MemoryMarshal.CreateReadOnlySpan(
                ref Unsafe.As<InlineBuffer, byte>(ref Unsafe.AsRef(in _payloadBuffer)),
                Unsafe.SizeOf<InlineBuffer>()
            );
            return MemoryMarshal.Read<T>(source);
        }
    }
}
