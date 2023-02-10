import { GuildAction } from "./GuildAction";
import { ResourceAmount } from "./ResourceAmount";
import { ResourceSet } from "./ResourceSet";

export interface Guild {
  productionRateCommon: number;
  productionRateRare: number;
  buildingCostCommon: ResourceSet;
  buildingCostRare: ResourceSet;
  buildingCountCommon: number;
  buildingCountRare: number;
  upgradeCostCommon: ResourceAmount;
  upgradeCostRare: ResourceAmount;
  action: GuildAction;
}