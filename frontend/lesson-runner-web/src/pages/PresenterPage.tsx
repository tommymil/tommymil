import { useParams } from "react-router-dom";
import { PresenterCockpit } from "../features/presenter/PresenterCockpit";

export function PresenterPage() {
  const { lessonId } = useParams();
  return <PresenterCockpit lessonId={lessonId} />;
}
