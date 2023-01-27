import { ObjectiveRequirement } from "./ObjectiveRequirement";

export interface Objective {
  requirements: ObjectiveRequirement[];
  queuedForCompletion: boolean;
}