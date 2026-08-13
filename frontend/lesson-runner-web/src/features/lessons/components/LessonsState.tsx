type LessonsStateProps = {
  message: string;
  tone?: "neutral" | "error";
};

export function LessonsState({ message, tone = "neutral" }: LessonsStateProps) {
  return <div className={`list-state list-state-${tone}`}>{message}</div>;
}
