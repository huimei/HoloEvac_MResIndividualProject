/*
 * Copyright (c) 2012-2015, Peter Abeles. All Rights Reserved.
 *
 * This file is part of DDogleg (http://ddogleg.org).
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

package org.ddogleg.struct;

import org.junit.Test;

import static org.junit.Assert.assertEquals;
import static org.junit.Assert.assertTrue;

/**
 * @author Peter Abeles
 */
public class TestRecycleManager {
	@Test
	public void requestInstance_recycleInstance() {
		RecycleManager<Dummy> manager = new RecycleManager<Dummy>(Dummy.class);

		Dummy first = manager.requestInstance();
		Dummy second = manager.requestInstance();
		manager.recycleInstance(first);
		Dummy third = manager.requestInstance();

		assertTrue(first==third);
		assertTrue(first!=second);
		assertEquals(0, manager.unused.size());
	}


	public static class Dummy {

	}
}
