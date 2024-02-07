import {serviceContainer} from "./lib/models/inversify_config";
import {IocTypes} from "./lib/models/ioc_types";
import {ICompositionalRoot} from "./lib/interfaces/composition_root";

(async () => {
    const compositionalRoot = serviceContainer.get<ICompositionalRoot>(IocTypes.ICompositionalRoot);
    await compositionalRoot.start();
})();