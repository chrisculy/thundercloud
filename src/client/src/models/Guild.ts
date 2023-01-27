import { GuildName } from "./GuildName";
import { ResourceAmount } from "./ResourceAmount";
import { ResourceSet } from "./ResourceSet";

export interface Guild {
  name: GuildName;
  productionRateCommon: number;
  productionRateRare: number;
  buildingCostCommon: ResourceSet;
  buildingCostRare: ResourceSet;
  buildingCountCommon: number;
  buildingCountRare: number;
  upgradeCostCommon: ResourceAmount;
  upgradeCostRare: ResourceAmount;
}