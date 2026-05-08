import { fourPartyFarmData, fourPartyFarmNote } from '../data/4p-farm'

export const tab4pFarm = {
  id: '4p-farm',
  label: '4 Party - Farm',
  rows: fourPartyFarmData,
  notes: fourPartyFarmNote,
  farmConfig: {
    firstInsertIndex: 56,
    secondInsertIndex: 67,
    equitesStableIndex: 54,
    equitesWarehouseUpdateIndex: 64,
  },
}
