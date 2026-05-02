import type { ComponentType } from "react";
import type { LucideProps } from "lucide-react";
import {
  ClipboardX,
  Frown,
  Ghost,
  Keyboard,
  MousePointer2,
  Move,
  Quote,
} from "lucide-react";

export const GREMLIN_IDS = [
  "the_drifter",
  "the_typist",
  "the_amnesiac",
  "the_critic",
  "the_philosopher",
  "the_lag_ghost",
  "the_rearranger",
] as const;

export type GremlinId = (typeof GREMLIN_IDS)[number];

const MAP: Record<GremlinId, ComponentType<LucideProps>> = {
  the_drifter: MousePointer2,
  the_typist: Keyboard,
  the_amnesiac: ClipboardX,
  the_critic: Frown,
  the_philosopher: Quote,
  the_lag_ghost: Ghost,
  the_rearranger: Move,
};

export function GremlinRoleIcon({
  id,
  className,
  size = 22,
  ...rest
}: { id: GremlinId; className?: string; size?: number } & Omit<
  LucideProps,
  "ref"
>) {
  const Cmp = MAP[id] ?? MousePointer2;
  return (
    <Cmp
      className={className}
      size={size}
      strokeWidth={2}
      aria-hidden
      {...rest}
    />
  );
}
