import { Reducer } from "redux";
import { ActionType, createAction, getType } from "typesafe-actions";
import { Guild } from "../models/Guild";
import { GuildAction } from "../models/GuildAction";
import { GuildName } from "../models/GuildName";
import { ResourceSet } from "../models/ResourceSet";

export const setActiveGuild = createAction('NATION_SET_ACTIVE_GUILD')<GuildName>();
export const setGuildAction = createAction('NATION_SET_GUILD_ACTION')<GuildName, GuildAction>();

const NationActions = {
	setActiveGuild,
  setGuildAction,
};

export type NationAction = ActionType<typeof NationActions>;

const defaultAgricultureGuild: Guild  = {
  name: "Agriculture",
  buildingCostCommon: {
    wood: 2,
    food: 1,
    power: 1,
  },
  buildingCostRare: {
    wood: 4,
    superfood: 2,
    power: 2,
  },
  buildingCountCommon: 0,
  buildingCountRare: 0,
  productionRateCommon: 1,
  productionRateRare: 1,
  upgradeCostCommon: {
    resource: "knowledge",
    amount: 1,
  },
  upgradeCostRare: {
    resource: "knowledge",
    amount: 2,
  },
};

const defaultIndustryGuild: Guild = {
  name: "Industry",
  buildingCostCommon: {
    wood: 2,
    superfood: 1,
    power: 1,
  },
  buildingCostRare: {
    stone: 2,
    superfood: 2,
    stormpower: 2,
  },
  buildingCountCommon: 0,
  buildingCountRare: 0,
  productionRateCommon: 1,
  productionRateRare: 1,
  upgradeCostCommon: {
    resource: "knowledge",
    amount: 1,
  },
  upgradeCostRare: {
    resource: "knowledge",
    amount: 3,
  },
};

const defaultResearchGuild: Guild = {
  name: "Research",
  buildingCostCommon: {
    wood: 2,
    food: 1,
    stormpower: 2,
  },
  buildingCostRare: {
    stone: 2,
    food: 2,
    stormpower: 4,
  },
  buildingCountCommon: 0,
  buildingCountRare: 0,
  productionRateCommon: 1,
  productionRateRare: 1,
  upgradeCostCommon: {
    resource: "knowledge",
    amount: 1,
  },
  upgradeCostRare: {
    resource: "stormknowledge",
    amount: 2,
  },
};

const defaultEnergyGuild: Guild = {
  name: "Energy",
  buildingCostCommon: {
    stone: 2,
    food: 1,
    power: 1,
  },
  buildingCostRare: {
    stone: 4,
    superfood: 2,
    stormpower: 2,
  },
  buildingCountCommon: 0,
  buildingCountRare: 0,
  productionRateCommon: 1,
  productionRateRare: 1,
  upgradeCostCommon: {
    resource: "stormknowledge",
    amount: 1,
  },
  upgradeCostRare: {
    resource: "stormknowledge",
    amount: 3,
  },
};

export type Guilds = {
  [guild in GuildName]: Guild;
};

export interface NationState {
  guilds: Guilds;
  activeGuild: GuildName;
  resources: ResourceSet;
};

const createDefaultNationState = (): NationState => {
  return {
    guilds: {
      "Agriculture": defaultAgricultureGuild,
      "Industry": defaultIndustryGuild,
      "Energy": defaultEnergyGuild,
      "Research": defaultResearchGuild,
    },
    activeGuild: "Agriculture",
    resources: {
      food: 0,
      knowledge: 0,
      power: 0,
      stone: 0,
      stormknowledge: 0,
      stormpower: 0,
      superfood: 0,
      wood: 0,
    }
  };
};

export const nationReducer: Reducer<NationState, NationAction> = (state: NationState = createDefaultNationState(), action: NationAction) => {
  switch (action.type) {
    case getType(NationActions.setActiveGuild):
      return { ...state };
    case getType(NationActions.setGuildAction):
      return { ...state, activeGuild: action.payload };
  }

  return state;
}