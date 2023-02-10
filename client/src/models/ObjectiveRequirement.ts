import { GuildName } from "./GuildName";
import { ObjectiveRequirementType } from "./ObjectiveRequirementType";
import { Resource } from "./Resource";


export interface ObjectiveRequirement {
  type: ObjectiveRequirementType;
  value: Resource | GuildName;
  amount: number;
}
