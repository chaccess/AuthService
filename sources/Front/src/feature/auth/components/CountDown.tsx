import { useEffect, useState } from "react";

export function useCountdown(initial: number) {
    const [secondsLeft, setSecondsLeft] = useState(initial);
    const [isRunning, setIsRunning] = useState(false);

    useEffect(() => {
        if (!isRunning) return;
        if (secondsLeft <= 0) {
            setIsRunning(false);
            return;
        }

        const id = setInterval(() => {
            setSecondsLeft((prev) => prev - 1);
        }, 1000);

        return () => clearInterval(id);
    }, [isRunning, secondsLeft]);

    const start = () => setIsRunning(true);
    const pause = () => setIsRunning(false);
    const reset = () => {
        setSecondsLeft(initial);
        setIsRunning(false);
    };

    return { secondsLeft, isRunning, start, pause, reset };
}
