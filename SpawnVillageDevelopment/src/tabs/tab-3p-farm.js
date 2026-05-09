import { threePartyFarmAdditionData, threePartyFarmData, threePartyFarmNote } from '../data/3p-farm'

export const tab3pFarm = {
  id: '3p-farm',
  label: '3 Party - Farm',
  rows: threePartyFarmData,
  notes: threePartyFarmNote,
  farmConfig: {
    firstInsertIndex: 50,
    secondInsertIndex: 61,
    equitesStableIndex: 49,
    equitesWarehouseUpdateIndex: 72,
  },
  cpBuildingConfig: {
    beforeInsertIndex: 75,
    afterInsertIndex: 76,
  },
  additionRows: threePartyFarmAdditionData,
}
